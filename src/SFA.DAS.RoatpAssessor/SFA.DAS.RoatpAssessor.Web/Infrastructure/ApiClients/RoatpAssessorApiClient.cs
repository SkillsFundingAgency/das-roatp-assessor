using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public class RoatpAssessorApiClient : ApiClientBase<RoatpAssessorApiClient>, IRoatpAssessorApiClient
    {
        public RoatpAssessorApiClient(string baseUri, ILogger<RoatpAssessorApiClient> logger, IRoatpApplicationTokenService tokenService) : base(new HttpClient { BaseAddress = new Uri(baseUri) }, logger)
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

        public async Task AssignAssessor(Guid applicationId, AssignAssessorApplicationRequest request)
        {
            await Post($"Assessor/Applications/{applicationId}/Assign", request);
        }

        public async Task<List<RoatpAssessorApplicationSummary>> GetInProgressApplications(string userId)
        {
            return await Get<List<RoatpAssessorApplicationSummary>>($"Assessor/Applications/{userId}/InProgress");
        }
    }
}
