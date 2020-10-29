using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public class RoatpAssessorApiClient : ApiClientBase<RoatpAssessorApiClient>, IRoatpAssessorApiClient
    {
        public RoatpAssessorApiClient(string baseUri, ILogger<RoatpAssessorApiClient> logger, IRoatpApplicationTokenService tokenService) : base(new HttpClient { BaseAddress = new Uri(baseUri) }, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(_httpClient.BaseAddress));
        }

        public async Task<bool> AssignAssessor(Guid applicationId, AssignAssessorCommand request)
        {
            var result = await Post($"Assessor/Applications/{applicationId}/Assign", request);

            return result == HttpStatusCode.OK;
        }

        public async Task<List<AssessorSequence>> GetAssessorSequences(Guid applicationId)
        {
            return await Get<List<AssessorSequence>>($"/Assessor/Applications/{applicationId}/Overview");
        }

        public async Task<AssessorPage> GetAssessorPage(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId)
        {
            AssessorPage assessorPage;

            if (string.IsNullOrEmpty(pageId))
            {
                assessorPage = await Get<AssessorPage>($"/Assessor/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page");
            }
            else
            {
                assessorPage = await Get<AssessorPage>($"/Assessor/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}");
            }

            return assessorPage;
        }

        public async Task<List<AssessorSector>> GetAssessorSectors(Guid applicationId, string userId)
        {
            return await Post<GetAssessorSectorsRequest, List<AssessorSector>>($"/Assessor/Applications/{applicationId}/Sectors", new GetAssessorSectorsRequest
            {
                UserId = userId
            });
        }

        public async Task<AssessorSectorDetails> GetAssessorSectorDetails(Guid applicationId, string pageId)
        {
            return await Get<AssessorSectorDetails>($"/Assessor/Applications/{applicationId}/SectorDetails/{pageId}");
        }

        public async Task<bool> SubmitAssessorPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string userId,
                                                    string userDisplayName, string status, string comment)
        {
            var result = await Post($"/Assessor/Applications/{applicationId}/SubmitPageReviewOutcome", new SubmitAssessorPageReviewOutcomeCommand
            {
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId,
                UserId = userId,
                UserDisplayName = userDisplayName,
                Status = status,
                Comment = comment
            });

            return result == HttpStatusCode.OK;
        }

        public async Task<AssessorPageReviewOutcome> GetAssessorPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber,
                                                                  string pageId, string userId)
        {
            return await Post<GetAssessorPageReviewOutcomeRequest, AssessorPageReviewOutcome>($"/Assessor/Applications/{applicationId}/GetPageReviewOutcome", new GetAssessorPageReviewOutcomeRequest
            {
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId,
                UserId = userId
            });
        }

        public async Task<List<AssessorPageReviewOutcome>> GetAssessorPageReviewOutcomesForSection(Guid applicationId, int sequenceNumber, int sectionNumber,
                                                                                       string userId)
        {
            return await Post<GetAssessorPageReviewOutcomesForSectionRequest, List<AssessorPageReviewOutcome>>($"/Assessor/Applications/{applicationId}/GetPageReviewOutcomesForSection", new GetAssessorPageReviewOutcomesForSectionRequest
            {
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                UserId = userId
            });
        }

        public async Task<List<AssessorPageReviewOutcome>> GetAllAssessorPageReviewOutcomes(Guid applicationId, string userId)
        {
            return await Post<GetAllAssessorPageReviewOutcomesRequest, List<AssessorPageReviewOutcome>>($"/Assessor/Applications/{applicationId}/GetAllPageReviewOutcomes", new GetAllAssessorPageReviewOutcomesRequest
            {
                UserId = userId
            });
        }

        public async Task<bool> UpdateAssessorReviewStatus(Guid applicationId, string userId, string status)
        {
            var result = await Post($"/Assessor/Applications/{applicationId}/UpdateAssessorReviewStatus", new UpdateAssessorReviewStatusCommand
            {
                UserId = userId,
                Status = status
            });

            return result == HttpStatusCode.OK;
        }
    }
}
