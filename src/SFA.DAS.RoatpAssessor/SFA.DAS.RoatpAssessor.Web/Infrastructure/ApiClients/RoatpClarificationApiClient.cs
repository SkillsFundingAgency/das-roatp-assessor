using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Infrastructure;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
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

        public async Task<SectorDetails> GetClarificationSectorDetails(Guid applicationId, string pageId)
        {
            return await Get<SectorDetails>($"/Clarification/Applications/{applicationId}/SectorDetails/{pageId}");
        }

        public async Task<bool> SubmitClarificationPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string userId,
                                                    string userName, string clarificationResponse, string status, string comment, IFormFileCollection clarificationFiles)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(sequenceNumber.ToString()), "SequenceNumber");
            content.Add(new StringContent(sectionNumber.ToString()), "SectionNumber");
            content.Add(new StringContent(pageId), "PageId");
            content.Add(new StringContent(userId), "UserId");
            content.Add(new StringContent(userName), "UserName");
            content.Add(new StringContent(status), "Status");
            if (!string.IsNullOrEmpty(comment))
            {
                content.Add(new StringContent(comment), "Comment");
            }
            if (!string.IsNullOrEmpty(clarificationResponse))
            {
                content.Add(new StringContent(clarificationResponse), "ClarificationResponse");
            }

            if (clarificationFiles != null && clarificationFiles.Any())
            {
                foreach (var file in clarificationFiles)
                {
                    var fileContent = new StreamContent(file.OpenReadStream())
                    { Headers = { ContentLength = file.Length, ContentType = new MediaTypeHeaderValue(file.ContentType) } };
                    content.Add(fileContent, file.FileName, file.FileName);
                }
            }

            try
            {
                using (var response = await _httpClient.PostAsync($"/Clarification/Applications/{applicationId}/SubmitPageReviewOutcome", content))
                {
                    return response.StatusCode == HttpStatusCode.OK;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when submitting ClarificationPageReviewOutcome for Application: {applicationId} | Page: {pageId}");
                return false;
            }  
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

        public async Task<HttpResponseMessage> DownloadFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string fileName)
        {
            return await GetResponse($"/Clarification/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/Download/{fileName}");
        }

        public async Task<HttpResponseMessage> DeleteFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string fileName)
        {
            return await DeleteResponse($"/Clarification/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/Delete/{fileName}");
        }
    }
}
