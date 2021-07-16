using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Outcome;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public class RoatpApplicationApiClient : ApiClientBase<RoatpApplicationApiClient>, IRoatpApplicationApiClient
    {
        public RoatpApplicationApiClient(HttpClient httpClient, ILogger<RoatpApplicationApiClient> logger, IRoatpApplicationTokenService tokenService) : base(httpClient, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(_httpClient.BaseAddress));
        }

        public async Task<ApplicationCounts> GetApplicationCounts(string userId, string searchTerm)
        {
            return await Get<ApplicationCounts>($"Assessor/Applications/{userId}?searchTerm={searchTerm}");
        }

        public async Task<List<AssessorApplicationSummary>> GetNewApplications(string userId, string searchTerm, string sortColumn, string sortOrder)
        {
            return await Get<List<AssessorApplicationSummary>>($"Assessor/Applications/{userId}/New?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}");
        }

        public async Task<List<AssessorApplicationSummary>> GetInProgressApplications(string userId, string searchTerm, string sortColumn, string sortOrder)
        {
            return await Get<List<AssessorApplicationSummary>>($"Assessor/Applications/{userId}/InProgress?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}");
        }

        public async Task<List<ModerationApplicationSummary>> GetInModerationApplications(string userId, string searchTerm, string sortColumn, string sortOrder)
        {
            return await Get<List<ModerationApplicationSummary>>($"Assessor/Applications/{userId}/InModeration?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}");
        }

        public async Task<List<ClarificationApplicationSummary>> GetInClarificationApplications(string userId, string searchTerm, string sortColumn, string sortOrder)
        {
            return await Get<List<ClarificationApplicationSummary>>($"Assessor/Applications/{userId}/InClarification?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}");
        }

        public async Task<List<ClosedApplicationSummary>> GetClosedApplications(string userId, string searchTerm, string sortColumn, string sortOrder)
        {
            return await Get<List<ClosedApplicationSummary>>($"Assessor/Applications/{userId}/Closed?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}");
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
