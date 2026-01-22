using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Refit;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpAssessorApiClient
    {
        [Post("/Assessor/Applications/{applicationId}/Assign")]
        Task<ApiResponse<object>> AssignAssessor(Guid applicationId, [Body] AssignAssessorCommand request);

        [Get("/Assessor/Applications/{applicationId}/Overview")]
        Task<List<AssessorSequence>> GetAssessorSequences(Guid applicationId);

        [Get("/Assessor/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}")]
        Task<AssessorPage> GetAssessorPage(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId);

        [Get("/Assessor/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page")]
        Task<AssessorPage> GetAssessorPage(Guid applicationId, int sequenceNumber, int sectionNumber);

        [Post("/Assessor/Applications/{applicationId}/Sectors")]
        Task<List<AssessorSector>> GetAssessorSectors(Guid applicationId, [Body] GetAssessorSectorsRequest request);

        [Get("/Assessor/Applications/{applicationId}/SectorDetails/{pageId}")]
        Task<SectorDetails> GetAssessorSectorDetails(Guid applicationId, string pageId);

        [Post("/Assessor/Applications/{applicationId}/SubmitPageReviewOutcome")]
        Task<ApiResponse<object>> SubmitAssessorPageReviewOutcome(Guid applicationId, [Body] SubmitAssessorPageReviewOutcomeCommand command);

        [Post("/Assessor/Applications/{applicationId}/GetPageReviewOutcome")]
        Task<AssessorPageReviewOutcome> GetAssessorPageReviewOutcome(Guid applicationId, [Body] GetAssessorPageReviewOutcomeRequest request);

        [Post("/Assessor/Applications/{applicationId}/GetAllPageReviewOutcomes")]
        Task<List<AssessorPageReviewOutcome>> GetAllAssessorPageReviewOutcomes(Guid applicationId, [Body] GetAllAssessorPageReviewOutcomesRequest request);

        [Post("/Assessor/Applications/{applicationId}/UpdateAssessorReviewStatus")]
        Task<HttpResponseMessage> UpdateAssessorReviewStatus(Guid applicationId, [Body] UpdateAssessorReviewStatusCommand command);
    }
}
