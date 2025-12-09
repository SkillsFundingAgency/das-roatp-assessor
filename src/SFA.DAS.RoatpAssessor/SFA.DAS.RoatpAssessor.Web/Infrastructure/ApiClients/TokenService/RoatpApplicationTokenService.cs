using System;
using System.Threading;
using Azure.Core;
using Azure.Identity;
using SFA.DAS.RoatpAssessor.Web.Settings;

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

            TokenCredential credential = new DefaultAzureCredential();

            var tokenRequest = new TokenRequestContext(
                new[] { $"{_configuration.RoatpApplicationApiAuthentication.Identifier}/.default" }
            );

            AccessToken token = credential.GetToken(tokenRequest, CancellationToken.None);

            return token.Token;
        }
    }
}
