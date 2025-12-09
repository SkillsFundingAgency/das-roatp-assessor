using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using SFA.DAS.RoatpAssessor.Web.Extensions;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public static class DependencyInjection
    {
        public static void ConfigureDependencyInjection(IServiceCollection services)
        {

            ClaimsIdentityExtensions.Logger = services.BuildServiceProvider().GetService<ILogger<ClaimsPrincipal>>();
        }
    }
}
