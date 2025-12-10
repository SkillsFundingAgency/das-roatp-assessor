using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using RestEase;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpAssessorApiClient
    {
        [Post("Assessor/Applications/{applicationId}/Assign")]
        Task<bool> AssignAssessor([Path] Guid applicationId, [Body] AssignAssessorCommand request);

        [Get("/Assessor/Applications/{applicationId}/Overview")]
        Task<List<AssessorSequence>> GetAssessorSequences([Path] Guid applicationId);

        [Get("/Assessor/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}")]
        Task<AssessorPage> GetAssessorPage([Path] Guid applicationId, [Path] int sequenceNumber, [Path] int sectionNumber, [Path] string pageId); //TESTS MIGHT BE BROKEN BY PUTTING LOGIC BEFORE CALL RATHER THAN WITHIN

        [Get("/Assessor/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page")]
        Task<AssessorPage> GetAssessorPage([Path] Guid applicationId, [Path] int sequenceNumber, [Path] int sectionNumber);

        [Post("/Assessor/Applications/{applicationId}/Sectors")]
        Task<List<AssessorSector>> GetAssessorSectors([Path] Guid applicationId, [Body] GetAssessorSectorsRequest command);

        [Get("/Assessor/Applications/{applicationId}/SectorDetails/{pageId}")]
        Task<SectorDetails> GetAssessorSectorDetails([Path] Guid applicationId, [Path] string pageId);

        [Post("/Assessor/Applications/{applicationId}/SubmitPageReviewOutcome")] 
        Task<bool> SubmitAssessorPageReviewOutcome([Path] Guid applicationId, [Body] SubmitAssessorPageReviewOutcomeCommand command);

        [Post("/Assessor/Applications/{applicationId}/GetPageReviewOutcome")] 
        Task<AssessorPageReviewOutcome> GetAssessorPageReviewOutcome([Path] Guid applicationId, [Body] GetAssessorPageReviewOutcomeRequest command);

        [Post("/Assessor/Applications/{applicationId}/GetAllPageReviewOutcomes")]
        Task<List<AssessorPageReviewOutcome>> GetAllAssessorPageReviewOutcomes([Path] Guid applicationId, [Body] GetAllAssessorPageReviewOutcomesRequest command);

        [Post("/Assessor/Applications/{applicationId}/UpdateAssessorReviewStatus")] 
        Task<bool> UpdateAssessorReviewStatus([Path] Guid applicationId, [Body] UpdateAssessorReviewStatusCommand command);
    }
}
