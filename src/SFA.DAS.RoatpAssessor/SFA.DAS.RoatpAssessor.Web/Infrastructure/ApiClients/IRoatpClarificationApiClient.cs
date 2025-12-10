using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RestEase;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpClarificationApiClient
    {
        [Get("/Clarification/Applications/{applicationId}/Overview")]
        Task<List<ClarificationSequence>> GetClarificationSequences([Path] Guid applicationId);

        [Get("/Clarification/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}")]
        Task<ClarificationPage> GetClarificationPage([Path] Guid applicationId, [Path] int sequenceNumber, [Path] int sectionNumber, [Path] string pageId);

        [Post("/Clarification/Applications/{applicationId}/Sectors")]
        Task<List<ClarificationSector>> GetClarificationSectors([Path] Guid applicationId, [Body] GetClarificationSectorsRequest command); 

        [Get("/Clarification/Applications/{applicationId}/SectorDetails/{pageId}")]
        Task<SectorDetails> GetClarificationSectorDetails([Path] Guid applicationId, [Path] string pageId);

        [Post("/Clarification/Applications/{applicationId}/SubmitPageReviewOutcome")]
        Task<bool> SubmitClarificationPageReviewOutcome(Guid applicationId, [Body] MultipartFormDataContent content);

        [Post("/Clarification/Applications/{applicationId}/GetAllPageReviewOutcomes")]
        Task<List<ClarificationPageReviewOutcome>> GetAllClarificationPageReviewOutcomes([Path] Guid applicationId, [Body] GetAllClarificationPageReviewOutcomesRequest command);

        [Post("/Clarification/Applications/{applicationId}/SubmitPageReviewOutcome")] 
        Task<ClarificationPageReviewOutcome> GetClarificationPageReviewOutcome([Path] Guid applicationId, [Body] GetClarificationPageReviewOutcomeRequest command);

        [Get("/Clarification/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/Download/{fileName}")]
        Task<HttpResponseMessage> DownloadFile([Path] Guid applicationId, [Path] int sequenceNumber, [Path] int sectionNumber, [Path] string pageId, [Path] string fileName);

        [Get("/Clarification/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/Delete/{fileName}")]
        Task<HttpResponseMessage> DeleteFile([Path] Guid applicationId, [Path] int sequenceNumber, [Path] int sectionNumber, [Path] string pageId, [Path] string fileName);
    }
}
