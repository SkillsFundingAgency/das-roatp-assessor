﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public class RoatpApplicationApiClient : ApiClientBase<RoatpApplicationApiClient>, IRoatpApplicationApiClient
    {
        public RoatpApplicationApiClient(HttpClient httpClient, ILogger<RoatpApplicationApiClient> logger, IRoatpApplicationTokenService tokenService) : base(httpClient, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(_httpClient.BaseAddress));
        }

        public async Task<RoatpAssessorSummary> GetAssessorSummary(string userId)
        {
            return await Get<RoatpAssessorSummary>($"Assessor/Applications/{userId}");
        }

        public async Task<List<RoatpAssessorApplicationSummary>> GetNewApplications(string userId)
        {
            return await Get<List<RoatpAssessorApplicationSummary>>($"Assessor/Applications/{userId}/New");
        }

        public async Task<List<RoatpAssessorApplicationSummary>> GetInProgressApplications(string userId)
        {
            return await Get<List<RoatpAssessorApplicationSummary>>($"Assessor/Applications/{userId}/InProgress");
        }

        public async Task<List<RoatpModerationApplicationSummary>> GetModerationApplications(string userId)
        {
            // Note: might be a good idea to filter by userId on the end point
            return await Get<List<RoatpModerationApplicationSummary>>($"Assessor/Applications/Moderation");
        }

        public async Task<Apply> GetApplication(Guid applicationId)
        {
            return await Get<Apply>($"/Application/{applicationId}");
        }

        public async Task<Contact> GetContactForApplication(Guid applicationId)
        {
            return await Get<Contact>($"/Application/{applicationId}/Contact");
        }

        public async Task<HttpResponseMessage> DownloadFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string questionId, string filename)
        {
            return await GetResponse($"/Assessor/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/Questions/{questionId}/download/{filename}");
        }
    }
}
