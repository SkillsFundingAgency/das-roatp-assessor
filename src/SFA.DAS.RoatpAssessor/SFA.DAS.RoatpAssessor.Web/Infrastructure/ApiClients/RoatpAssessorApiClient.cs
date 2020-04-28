﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public class RoatpAssessorApiClient : ApiClientBase<RoatpAssessorApiClient>, IRoatpAssessorApiClient
    {
        private HttpClient _client;

        public RoatpAssessorApiClient(string baseUri, ILogger<RoatpAssessorApiClient> logger, IRoatpApplicationTokenService tokenService) : base(new HttpClient { BaseAddress = new Uri(baseUri) }, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(_httpClient.BaseAddress));
        }

        public async Task<List<RoatpAssessorApplicationSummary>> GetNewApplications(string userId)
        {
            return await Get<List<RoatpAssessorApplicationSummary>>($"Assessor/Applications/{userId}/New");
        }
    }
}