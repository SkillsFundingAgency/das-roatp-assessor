using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using SFA.DAS.RoatpAssessor.Web.Settings;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService
{
    public class RoatpApplicationTokenService : IRoatpApplicationTokenService
    {
        private readonly IWebConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        public RoatpApplicationTokenService(IWebConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public string GetToken()
        {
            if (_hostingEnvironment.IsDevelopment())
                return string.Empty;

            var tenantId = _configuration.RoatpApplicationApiAuthentication.TenantId;
            var clientId = _configuration.RoatpApplicationApiAuthentication.ClientId;
            var appKey = _configuration.RoatpApplicationApiAuthentication.ClientSecret;
            var resourceId = _configuration.RoatpApplicationApiAuthentication.ResourceId;

            var authority = $"https://login.microsoftonline.com/{tenantId}";
            var clientCredential = new ClientCredential(clientId, appKey);
            var context = new AuthenticationContext(authority, true);
            var result = context.AcquireTokenAsync(resourceId, clientCredential).Result;

            return result.AccessToken;
        }
    }
}
