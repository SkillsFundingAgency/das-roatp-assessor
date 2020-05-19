using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.Models;

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

        public async Task<List<AssessorSequence>> GetAssessorSequences(Guid applicationId)
        {
            return await Get<List<AssessorSequence>>($"/Assessor/Applications/{applicationId}/Overview");
        }

        public async Task<List<Sector>> GetChosenSectors(Guid applicationId)
        {
            return await Get<List<Sector>>($"/Assessor/Applications/ChosenSectors/{applicationId}");
        }

        public async Task<AssessorPage> GetAssessorPage(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId)
        {
            AssessorPage assessorPage;

            if(string.IsNullOrEmpty(pageId))
            {
                assessorPage = await Get<AssessorPage>($"/Assessor/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page");
            }
            else
            {
                assessorPage = await Get<AssessorPage>($"/Assessor/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}");
            }

            return assessorPage;
        }

        public async Task SubmitAssessorPageOutcome(Guid applicationId, 
                                                    int sequenceNumber, 
                                                    int sectionNumber, 
                                                    string pageId, 
                                                    int assessorType,  
                                                    string userId, 
                                                    string status,
                                                    string comment)
        {
            _logger.LogInformation($"RoatpApplicationApiClient-SubmitAssessorPageOutcome - ApplicationId '{applicationId}' - " +
                                                    $"SequenceNumber '{sequenceNumber}' - SectionNumber '{sectionNumber}' - PageId '{pageId}' - " +
                                                    $"AssessorType '{assessorType}' - UserId '{userId}' - " +
                                                    $"Status '{status}' - Comment '{comment}'");

            try
            {
                var response = await Post($"/Assessor/SubmitPageOutcome", new 
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

                _logger.LogInformation($"RoatpApplicationApiClient-SubmitAssessorPageOutcome - ResponseStatusCode '{response}'"); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpApplicationApiClient-SubmitAssessorPageOutcome - Error: '" + ex.Message + "'");
            }
        }

        public async Task<PageReviewOutcome> GetPageReviewOutcome(Guid applicationId,
                                                    int sequenceNumber,
                                                    int sectionNumber,
                                                    string pageId,
                                                    int assessorType,
                                                    string userId)
        {
            _logger.LogInformation($"RoatpApplicationApiClient-SubmitAssessorPageOutcome - ApplicationId '{applicationId}' - " +
                                                    $"SequenceNumber '{sequenceNumber}' - SectionNumber '{sectionNumber}' - PageId '{pageId}' - " +
                                                    $"AssessorType '{assessorType}' - UserId '{userId}'");
            try
            {
                var pageReviewOutcome = await Post<GetPageReviewOutcomeRequest, PageReviewOutcome>($"/Assessor/GetPageReviewOutcome", new GetPageReviewOutcomeRequest
                {
                    ApplicationId = applicationId,
                    SequenceNumber = sequenceNumber,
                    SectionNumber = sectionNumber,
                    PageId = pageId,
                    AssessorType = assessorType,
                    UserId = userId
                });

                return pageReviewOutcome;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpApplicationApiClient-SubmitAssessorPageOutcome - Error: '" + ex.Message + "'");
            }

            return new PageReviewOutcome();
        }

        public async Task<List<PageReviewOutcome>> GetAssessorReviewOutcomesPerSection(Guid applicationId,
                                                    int sequenceNumber,
                                                    int sectionNumber,
                                                    int assessorType,
                                                    string userId)
        {
            _logger.LogInformation($"RoatpApplicationApiClient-GetAssessorReviewOutcomesPerSection - ApplicationId '{applicationId}' - " +
                                                    $"SequenceNumber '{sequenceNumber}' - SectionNumber '{sectionNumber}' - " +
                                                    $"AssessorType '{assessorType}' - UserId '{userId}'");
            try
            {
                var assessorReviewOutcomes = await Post<GetAssessorReviewOutcomesPerSectionRequest, List<PageReviewOutcome>>($"/Assessor/GetAssessorReviewOutcomesPerSection", new GetAssessorReviewOutcomesPerSectionRequest
                {
                    ApplicationId = applicationId,
                    SequenceNumber = sequenceNumber,
                    SectionNumber = sectionNumber,
                    AssessorType = assessorType,
                    UserId = userId
                });

                return assessorReviewOutcomes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpApplicationApiClient-SubmitAssessorPageOutcome - Error: '" + ex.Message + "'");
            }

            return new List<PageReviewOutcome>();
        }

        public async Task<List<PageReviewOutcome>> GetAllAssessorReviewOutcomes(Guid applicationId,
                                                    int assessorType,
                                                    string userId)
        {
            _logger.LogInformation($"RoatpApplicationApiClient-GetAllAssessorReviewOutcomes - ApplicationId '{applicationId}' - " +
                                                    $"AssessorType '{assessorType}' - UserId '{userId}'");
            try
            {
                var assessorReviewOutcomes = await Post<GetAllAssessorReviewOutcomesRequest, List<PageReviewOutcome>>($"/Assessor/GetAllAssessorReviewOutcomes", new GetAllAssessorReviewOutcomesRequest
                {
                    ApplicationId = applicationId,
                    AssessorType = assessorType,
                    UserId = userId
                });

                return assessorReviewOutcomes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RoatpApplicationApiClient-GetAllAssessorReviewOutcomes - Error: '" + ex.Message + "'");
            }

            return new List<PageReviewOutcome>();
        }

        public async Task<HttpResponseMessage> DownloadFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string questionId, string filename)
        {
            return await GetResponse($"/Assessor/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/Questions/{questionId}/download/{filename}");
        }
    }
}
