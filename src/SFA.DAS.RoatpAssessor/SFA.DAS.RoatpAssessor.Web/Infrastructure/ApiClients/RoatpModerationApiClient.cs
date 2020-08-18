using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public class RoatpModerationApiClient : ApiClientBase<RoatpModerationApiClient>, IRoatpModerationApiClient
    {
        public RoatpModerationApiClient(string baseUri, ILogger<RoatpModerationApiClient> logger, IRoatpApplicationTokenService tokenService) : base(new HttpClient { BaseAddress = new Uri(baseUri) }, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(_httpClient.BaseAddress));
        }

    }
}
