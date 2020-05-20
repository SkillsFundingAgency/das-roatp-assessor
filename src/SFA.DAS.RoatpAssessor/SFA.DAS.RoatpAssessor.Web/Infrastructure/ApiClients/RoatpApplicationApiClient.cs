using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.AdminService.Common.Infrastructure;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public class RoatpApplicationApiClient : ApiClientBase<RoatpApplicationApiClient>, IRoatpApplicationApiClient
    {
        public RoatpApplicationApiClient(HttpClient httpClient, ILogger<RoatpApplicationApiClient> logger, IRoatpApplicationTokenService tokenService) : base(httpClient, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(_httpClient.BaseAddress));
        }

        public async Task<Apply> GetApplication(Guid applicationId)
        {
            return await Get<Apply>($"/Application/{applicationId}");
        }

        public async Task<Contact> GetContactForApplication(Guid applicationId)
        {
            return await Get<Contact>($"/Application/{applicationId}/Contact");
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

        public async Task<HttpResponseMessage> DownloadFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string questionId, string filename)
        {
            return await GetResponse($"/Assessor/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/Questions/{questionId}/download/{filename}");
        }
    }
}
