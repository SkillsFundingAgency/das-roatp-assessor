using System;
using System.Collections.Generic;
using System.Net;
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

        public async Task<ClarificationPage> GetClarificationPage(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId)
        {
            ClarificationPage clarificationPage;

            if (string.IsNullOrEmpty(pageId))
            {
                clarificationPage = await Get<ClarificationPage>($"/Clarification/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page");
            }
            else
            {
                clarificationPage = await Get<ClarificationPage>($"/Clarification/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}");
            }

            return clarificationPage;
        }

        public async Task<List<ClarificationSector>> GetClarificationSectors(Guid applicationId, string userId)
        {
            return await Post<GetClarificationSectorsRequest, List<ClarificationSector>>($"/Clarification/Applications/{applicationId}/Sectors", new GetClarificationSectorsRequest
            {
                UserId = userId
            });
        }

        public async Task<ClarificationSectorDetails> GetClarificationSectorDetails(Guid applicationId, string pageId)
        {
            return await Get<ClarificationSectorDetails>($"/Clarification/Applications/{applicationId}/SectorDetails/{pageId}");
        }

        public async Task<ModerationOutcome> GetModerationOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId)
        {
            return await Get<ModerationOutcome>($"/Clarification/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/ModerationOutcome");
        }

        public async Task<bool> SubmitClarificationPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId,
                                                    string userId, string clarificationResponse, string status, string comment)
        {
            var result = await Post($"/Clarification/Applications/{applicationId}/SubmitPageReviewOutcome", new SubmitClarificationPageReviewOutcomeCommand
            {
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId,
                UserId = userId,
                Status = status,
                Comment = comment,
                ClarificationResponse = clarificationResponse
            });

            return result == HttpStatusCode.OK;
        }

        public async Task<ClarificationPageReviewOutcome> GetClarificationPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string userId)
        {
            return await Post<GetClarificationPageReviewOutcomeRequest, ClarificationPageReviewOutcome>($"/Clarification/Applications/{applicationId}/GetPageReviewOutcome", new GetClarificationPageReviewOutcomeRequest
            {
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId,
                UserId = userId
            });
        }

        public async Task<List<ClarificationPageReviewOutcome>> GetClarificationPageReviewOutcomesForSection(Guid applicationId, int sequenceNumber, int sectionNumber, string userId)
        {
            return await Post<GetClarificationPageReviewOutcomesForSectionRequest, List<ClarificationPageReviewOutcome>>($"/Clarification/Applications/{applicationId}/GetPageReviewOutcomesForSection", new GetClarificationPageReviewOutcomesForSectionRequest
            {
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                UserId = userId
            });
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
