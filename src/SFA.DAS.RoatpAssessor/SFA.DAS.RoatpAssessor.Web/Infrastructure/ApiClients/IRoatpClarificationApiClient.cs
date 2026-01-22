using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Refit;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpClarificationApiClient
    {
        [Get("/Clarification/Applications/{applicationId}/Overview")]
        Task<List<ClarificationSequence>> GetClarificationSequences(Guid applicationId);

        [Get("/Clarification/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}")]
        Task<ClarificationPage> GetClarificationPage(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId);

        [Get("/Clarification/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page")]
        Task<ClarificationPage> GetClarificationPage(Guid applicationId, int sequenceNumber, int sectionNumber);

        [Post("/Clarification/Applications/{applicationId}/Sectors")]
        Task<List<ClarificationSector>> GetClarificationSectors(Guid applicationId, [Body] GetClarificationSectorsRequest request);

        [Get("/Clarification/Applications/{applicationId}/SectorDetails/{pageId}")]
        Task<SectorDetails> GetClarificationSectorDetails(Guid applicationId, string pageId);

        [Post("/Clarification/Applications/{applicationId}/SubmitPageReviewOutcome")]
        Task<ApiResponse<object>> SubmitClarificationPageReviewOutcome(Guid applicationId, [Body] MultipartFormDataContent requestContent);

        [Post("/Clarification/Applications/{applicationId}/GetAllPageReviewOutcomes")]
        Task<List<ClarificationPageReviewOutcome>> GetAllClarificationPageReviewOutcomes(Guid applicationId, [Body] GetAllClarificationPageReviewOutcomesRequest request);

        [Post("/Clarification/Applications/{applicationId}/GetPageReviewOutcome")]
        Task<ClarificationPageReviewOutcome> GetClarificationPageReviewOutcome(Guid applicationId, [Body] GetClarificationPageReviewOutcomeRequest request);
        
        [Get("/Clarification/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/Download/{fileName}")]
        Task<HttpResponseMessage> DownloadFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string fileName);

        [Get("/Clarification/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/Delete/{fileName}")]
        Task<HttpResponseMessage> DeleteFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string fileName);
    }
}
