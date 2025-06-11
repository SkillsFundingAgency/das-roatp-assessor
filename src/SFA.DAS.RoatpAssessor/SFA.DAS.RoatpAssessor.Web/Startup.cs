﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Primitives;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.AdminService.Common;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;
using SFA.DAS.RoatpAssessor.Web.ModelBinders;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Settings;
using SFA.DAS.RoatpAssessor.Web.StartupExtensions;
using SFA.DAS.RoatpAssessor.Web.Validators;

namespace SFA.DAS.RoatpAssessor.Web
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        private const string Culture = "en-GB";

        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;
        private readonly ILogger<Startup> _logger;

        public IWebConfiguration ApplicationConfiguration { get; set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env, ILogger<Startup> logger)
        {
            _env = env;
            _logger = logger;

            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration)
                .SetBasePath(Directory.GetCurrentDirectory());
#if DEBUG
            if (!configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                config.AddJsonFile("appsettings.json", true)
                    .AddJsonFile("appsettings.Development.json", true);
            }
#endif
            config.AddEnvironmentVariables();

            if (!configuration["EnvironmentName"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase))
            {
                config.AddAzureTableStorage(options =>
                    {
                        options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                        options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                        options.EnvironmentName = configuration["EnvironmentName"];
                        options.PreFixConfigurationKeys = false;
                    }
                );
            }

            _configuration = config.Build();
            ApplicationConfiguration = _configuration.GetSection(nameof(WebConfiguration)).Get<WebConfiguration>();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false; // Default is true, make it false
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            AddAuthentication(services);

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(Culture);
                options.SupportedCultures = new List<CultureInfo> { new CultureInfo(Culture) };
                options.RequestCultureProviders.Clear();
            });

            services.AddMvc(options =>
                {
                    //options.Filters.Add<CheckSessionFilter>();
                    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                    options.ModelBinderProviders.Insert(0, new StringTrimmingModelBinderProvider());
                })
                // NOTE: Can we move this to 2.2 to match the version of .NET Core we're coding against?
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1).AddJsonOptions(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.AddSession(opt => { opt.IdleTimeout = TimeSpan.FromHours(1); });

            services.AddCache(ApplicationConfiguration, _env);
            services.AddDataProtection(ApplicationConfiguration, _env);

            AddAntiforgery(services);

            services.AddHealthChecks();

            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
                loggingBuilder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Information);
            });

            services.AddApplicationInsightsTelemetry();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

            ConfigureHttpClients(services);
            ConfigureDependencyInjection(services);
        }

        private void AddAuthentication(IServiceCollection services)
        {
            if (ApplicationConfiguration.UseDfeSignIn)
            {
                services.AddAndConfigureDfESignInAuthentication(_configuration,
                    "SFA.DAS.AdminService.Web.Auth",
                    typeof(CustomServiceRole),
                    ClientName.RoatpServiceAdmin,
                    "/SignOut",
                    "");
            }
            else
            {
                services.AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = WsFederationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = WsFederationDefaults.AuthenticationScheme;
                }).AddWsFederation(options =>
                {
                    options.Wtrealm = ApplicationConfiguration.StaffAuthentication.WtRealm;
                    options.MetadataAddress = ApplicationConfiguration.StaffAuthentication.MetadataAddress;
                    options.TokenValidationParameters.RoleClaimType = Roles.RoleClaimType;
                }).AddCookie();
            }
        }

        private void AddAntiforgery(IServiceCollection services)
        {
            services.AddAntiforgery(options => options.Cookie = new CookieBuilder() { Name = ".RoatpAssessor.Staff.AntiForgery", HttpOnly = false });
        }

        private void ConfigureHttpClients(IServiceCollection services)
        {
            var acceptHeaderName = "Accept";
            var acceptHeaderValue = "application/json";
            var handlerLifeTime = TimeSpan.FromMinutes(5);

            services.AddHttpClient<IRoatpApplicationApiClient, RoatpApplicationApiClient>(config =>
            {
                config.BaseAddress = new Uri(ApplicationConfiguration.RoatpApplicationApiAuthentication.ApiBaseAddress);
                config.DefaultRequestHeaders.Add(acceptHeaderName, acceptHeaderValue);
            })
            .SetHandlerLifetime(handlerLifeTime)
            .AddPolicyHandler(GetRetryPolicy());
        }

        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddTransient(x => ApplicationConfiguration);

            services.AddTransient<IAssessorDashboardOrchestrator, AssessorDashboardOrchestrator>();
            services.AddTransient<IModeratorDashboardOrchestrator, ModeratorDashboardOrchestrator>();
            services.AddTransient<IClarificationDashboardOrchestrator, ClarificationDashboardOrchestrator>();
            services.AddTransient<IOutcomeDashboardOrchestrator, OutcomeDashboardOrchestrator>();

            services.AddTransient<ISearchTermValidator, SearchTermValidator>();

            services.AddTransient<IRoatpApplicationTokenService, RoatpApplicationTokenService>();
            services.AddTransient<IClarificationOutcomeOrchestrator, ClarificationOutcomeOrchestrator>();
            services.AddTransient<IRoatpAssessorApiClient>(x => new RoatpAssessorApiClient(
                ApplicationConfiguration.RoatpApplicationApiAuthentication.ApiBaseAddress,
                x.GetService<ILogger<RoatpAssessorApiClient>>(),
                x.GetService<IRoatpApplicationTokenService>()));

            services.AddTransient<IRoatpModerationApiClient>(x => new RoatpModerationApiClient(
                ApplicationConfiguration.RoatpApplicationApiAuthentication.ApiBaseAddress,
                x.GetService<ILogger<RoatpModerationApiClient>>(),
                x.GetService<IRoatpApplicationTokenService>()));

            services.AddTransient<IRoatpClarificationApiClient>(x => new RoatpClarificationApiClient(
                ApplicationConfiguration.RoatpApplicationApiAuthentication.ApiBaseAddress,
                x.GetService<ILogger<RoatpClarificationApiClient>>(),
                x.GetService<IRoatpApplicationTokenService>()));

            services.AddTransient<IAssessorOverviewOrchestrator, AssessorOverviewOrchestrator>();
            services.AddTransient<IModeratorOverviewOrchestrator, ModeratorOverviewOrchestrator>();
            services.AddTransient<IClarificationOverviewOrchestrator, ClarificationOverviewOrchestrator>();
            services.AddTransient<IOutcomeOverviewOrchestrator, OutcomeOverviewOrchestrator>();

            services.AddTransient<ISupplementaryInformationService, SupplementaryInformationService>();

            services.AddTransient<IAssessorSectionReviewOrchestrator, AssessorSectionReviewOrchestrator>();
            services.AddTransient<IAssessorPageValidator, AssessorPageValidator>();
            services.AddTransient<IAssessorOutcomeValidator, AssessorOutcomeValidator>();

            services.AddTransient<IModeratorSectionReviewOrchestrator, ModeratorSectionReviewOrchestrator>();
            services.AddTransient<IModeratorPageValidator, ModeratorPageValidator>();
            services.AddTransient<IModeratorOutcomeValidator, ModeratorOutcomeValidator>();
            services.AddTransient<IModeratorOutcomeOrchestrator, ModeratorOutcomeOrchestrator>();

            services.AddTransient<IClarificationSectionReviewOrchestrator, ClarificationSectionReviewOrchestrator>();
            services.AddTransient<IClarificationPageValidator, ClarificationPageValidator>();
            services.AddTransient<IClarificationOutcomeValidator, ClarificationOutcomeValidator>();

            services.AddTransient<IOutcomeSectionReviewOrchestrator, OutcomeSectionReviewOrchestrator>();

            DependencyInjection.ConfigureDependencyInjection(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseCookiePolicy();

            app.UseSession();
            app.UseRequestLocalization();
            app.UseStatusCodePagesWithReExecute("/ErrorPage/{0}");
            app.UseSecurityHeaders();
            app.Use(async (context, next) =>
            {
                if (!context.Response.Headers.ContainsKey("X-Permitted-Cross-Domain-Policies"))
                {
                    context.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", new StringValues("none"));
                }
                await next();
            });
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseHealthChecks("/health");
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }
    }
}
