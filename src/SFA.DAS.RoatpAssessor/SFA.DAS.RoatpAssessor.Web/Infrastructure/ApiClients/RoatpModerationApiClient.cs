using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public class RoatpModerationApiClient : ApiClientBase<RoatpModerationApiClient>, IRoatpModerationApiClient
    {
        public RoatpModerationApiClient(string baseUri, ILogger<RoatpModerationApiClient> logger, IRoatpApplicationTokenService tokenService) : base(new HttpClient { BaseAddress = new Uri(baseUri) }, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(_httpClient.BaseAddress));
        }

        public async Task<List<ModeratorSequence>> GetModeratorSequences(Guid applicationId)
        {
            return await Get<List<ModeratorSequence>>($"/Moderator/Applications/{applicationId}/Overview");
        }

        public async Task<ModeratorPageReviewOutcome> GetModeratorPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string userId)
        {
            return await Post<GetModeratorPageReviewOutcomeRequest, ModeratorPageReviewOutcome>($"/Moderator/Applications/{applicationId}/GetPageReviewOutcome", new GetModeratorPageReviewOutcomeRequest
            {
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId,
                UserId = userId
            });
        }

        public async Task<List<ModeratorPageReviewOutcome>> GetModeratorPageReviewOutcomesForSection(Guid applicationId, int sequenceNumber, int sectionNumber, string userId)
        {
            return await Post<GetModeratorPageReviewOutcomesForSectionRequest, List<ModeratorPageReviewOutcome>>($"/Moderator/Applications/{applicationId}/GetPageReviewOutcomesForSection", new GetModeratorPageReviewOutcomesForSectionRequest
            {
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                UserId = userId
            });
        }

        public async Task<List<ModeratorPageReviewOutcome>> GetAllModeratorPageReviewOutcomes(Guid applicationId, string userId)
        {
            return await Post<GetAllModeratorPageReviewOutcomesRequest, List<ModeratorPageReviewOutcome>>($"/Moderator/Applications/{applicationId}/GetAllPageReviewOutcomes", new GetAllModeratorPageReviewOutcomesRequest
            {
                UserId = userId
            });
        }

        public async Task<List<ModeratorSector>> GetModeratorSectors(Guid applicationId, string userId)
        {
            return await Get<List<ModeratorSector>>($"/Moderator/Applications/{applicationId}/ChosenSectors/user/{userId}");
        }
    }
}
