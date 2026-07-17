using System;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.RoatpAssessor.Web.Settings;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService
{
    public class RoatpApplicationTokenService(IAzureClientCredentialHelper _azureClientCredentialHelper, IWebConfiguration _configuration) : IRoatpApplicationTokenService
    {
        public string GetToken(Uri baseUri)
        {
            if (baseUri != null && baseUri.IsLoopback)
                return string.Empty;

            var generateTokenTask = _azureClientCredentialHelper.GetAccessTokenAsync(_configuration.RoatpApplicationApiAuthentication.Identifier);

            return generateTokenTask.GetAwaiter().GetResult();
        }
    }
}
