using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Polly;
using Polly.Extensions.Http;
using Polly.Retry;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Enums;
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
        private readonly IWebHostEnvironment _env;

        public IWebConfiguration ApplicationConfiguration { get; set; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            _env = env;

            var config = new ConfigurationBuilder()
                .AddConfiguration(configuration);

            config.AddAzureTableStorage(options =>
                {
                    options.ConfigurationKeys = configuration["ConfigNames"].Split(",");
                    options.StorageConnectionString = configuration["ConfigurationStorageConnectionString"];
                    options.EnvironmentName = configuration["EnvironmentName"];
                    options.PreFixConfigurationKeys = false;
                }
            );

            _configuration = config.Build();
            ApplicationConfiguration = _configuration.Get<WebConfiguration>();
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

            services.AddAndConfigureDfESignInAuthentication(_configuration,
                "SFA.DAS.AdminService.Web.Auth",
                typeof(CustomServiceRole),
                ClientName.RoatpServiceAdmin,
                "/SignOut",
                "");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(Culture);
                options.SupportedCultures = [new(Culture)];
                options.RequestCultureProviders.Clear();
            });

            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.ModelBinderProviders.Insert(0, new StringTrimmingModelBinderProvider());
            });

            services.AddSession(opt => { opt.IdleTimeout = TimeSpan.FromHours(1); });

            services.AddCache(ApplicationConfiguration, _env);
            services.AddDataProtection(ApplicationConfiguration, _env);

            AddAntiforgery(services);

            services.AddHealthChecks();

            services.AddOpenTelemetryRegistration(_configuration["APPINSIGHTS_CONNECTION_STRING"]);

            ConfigureHttpClients(services);
            ConfigureDependencyInjection(services);
        }

        private static void AddAntiforgery(IServiceCollection services)
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
            app.UseRouting();
            app.UseSession();
            app.UseRequestLocalization();
            app.UseStatusCodePagesWithReExecute("/ErrorPage/{0}");
            app.UseSecurityHeaders();
            app.Use(async (context, next) =>
            {
                if (!context.Response.Headers.ContainsKey("X-Permitted-Cross-Domain-Policies"))
                {
                    context.Response.Headers.Append("X-Permitted-Cross-Domain-Policies", new StringValues("none"));
                }
                await next();
            });
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHealthChecks("/health");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller=Home}/{action=Index}/{id?}");
            });
        }

        static AsyncRetryPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }
    }
}
