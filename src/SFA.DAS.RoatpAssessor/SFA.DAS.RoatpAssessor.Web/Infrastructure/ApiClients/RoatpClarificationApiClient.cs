using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public class RoatpClarificationApiClient : ApiClientBase<RoatpClarificationApiClient>, IRoatpClarificationApiClient
    {
        public RoatpClarificationApiClient(string baseUri, ILogger<RoatpClarificationApiClient> logger, IRoatpApplicationTokenService tokenService) : base(new HttpClient { BaseAddress = new Uri(baseUri) }, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(_httpClient.BaseAddress));
        }

        public async Task<List<ClarificationSequence>> GetClarificationSequences(Guid applicationId)
        {
            return await Get<List<ClarificationSequence>>($"/Clarification/Applications/{applicationId}/Overview");
        }

        public async Task<List<ClarificationPageReviewOutcome>> GetAllClarificationPageReviewOutcomes(Guid applicationId, string userId)
        {
            return await Post<GetAllClarificationPageReviewOutcomesRequest, List<ClarificationPageReviewOutcome>>($"/Clarification/Applications/{applicationId}/GetAllPageReviewOutcomes", new GetAllClarificationPageReviewOutcomesRequest
            {
                UserId = userId
            });
        }
    }
}
