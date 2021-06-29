using Microsoft.Azure.Services.AppAuthentication;
using SFA.DAS.RoatpAssessor.Web.Settings;
using System;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService
{
    public class RoatpApplicationTokenService : IRoatpApplicationTokenService
    {
        private readonly IWebConfiguration _configuration;

        public RoatpApplicationTokenService(IWebConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetToken(Uri baseUri)
        {
            if (baseUri != null && baseUri.IsLoopback)
                return string.Empty;

            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var generateTokenTask = azureServiceTokenProvider.GetAccessTokenAsync(_configuration.RoatpApplicationApiAuthentication.Identifier);

            return generateTokenTask.GetAwaiter().GetResult();
        }
    }
}
