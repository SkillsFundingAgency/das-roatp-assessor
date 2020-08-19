using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.Domain;
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

        public async Task AssignAssessor(Guid applicationId, AssignAssessorApplicationRequest request)
        {
            await Post($"Assessor/Applications/{applicationId}/Assign", request);
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

        public async Task<List<Sector>> GetChosenSectors(Guid applicationId, string userId)
        {
            return await Get<List<Sector>>($"/Assessor/Applications/{applicationId}/ChosenSectors/user/{userId}");
        }

        public async Task<SectorDetails> GetSectorDetails(Guid applicationId, string pageId)
        {
            return await Get<SectorDetails>($"/Assessor/Applications/{applicationId}/SectorDetails/{pageId}");
        }

        public async Task<bool> SubmitAssessorPageOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId,
                                                    int assessorType, string userId, string status, string comment)
        {
            var result = await Post($"/Assessor/SubmitPageOutcome", new
            {
                applicationId,
                sequenceNumber,
                sectionNumber,
                pageId,
                assessorType,
                userId,
                status,
                comment
            });

            return result == HttpStatusCode.OK;
        }

        public async Task<PageReviewOutcome> GetPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber,
                                                                  string pageId, int assessorType, string userId)
        {
            return await Post<GetPageReviewOutcomeRequest, PageReviewOutcome>($"/Assessor/GetPageReviewOutcome", new GetPageReviewOutcomeRequest
            {
                ApplicationId = applicationId,
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId,
                AssessorType = assessorType,
                UserId = userId
            });
        }

        public async Task<List<PageReviewOutcome>> GetAssessorReviewOutcomesPerSection(Guid applicationId, int sequenceNumber, int sectionNumber,
                                                                                       int assessorType, string userId)
        {
            return await Post<GetAssessorReviewOutcomesPerSectionRequest, List<PageReviewOutcome>>($"/Assessor/GetAssessorReviewOutcomesPerSection", new GetAssessorReviewOutcomesPerSectionRequest
            {
                ApplicationId = applicationId,
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                AssessorType = assessorType,
                UserId = userId
            });
        }

        public async Task<List<PageReviewOutcome>> GetAllAssessorReviewOutcomes(Guid applicationId, int assessorType, string userId)
        {
            return await Post<GetAllAssessorReviewOutcomesRequest, List<PageReviewOutcome>>($"/Assessor/GetAllAssessorReviewOutcomes", new GetAllAssessorReviewOutcomesRequest
            {
                ApplicationId = applicationId,
                AssessorType = assessorType,
                UserId = userId
            });
        }

        public async Task<bool> UpdateAssessorReviewStatus(Guid applicationId, int assessorType, string userId, string status)
        {
            var result = await Post($"/Assessor/UpdateAssessorReviewStatus", new
            {
                applicationId,
                assessorType,
                userId,
                status
            });

            return result == HttpStatusCode.OK;
        }
    }
}
