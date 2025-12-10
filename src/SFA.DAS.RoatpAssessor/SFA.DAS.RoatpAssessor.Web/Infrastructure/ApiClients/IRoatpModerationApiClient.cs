using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpModerationApiClient
    {
        [Get("/Moderator/Applications/{applicationId}/Overview")]
        Task<List<ModeratorSequence>> GetModeratorSequences([Path] Guid applicationId);

        [Get("/Moderator/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}")]
        Task<ModeratorPage> GetModeratorPage([Path] Guid applicationId, [Path] int sequenceNumber, [Path] int sectionNumber, [Path] string pageId);  //TESTS MIGHT BE BROKEN BY PUTTING LOGIC BEFORE CALL RATHER THAN WITHIN

        [Get("/Moderator/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page")]
        Task<ModeratorPage> GetModeratorPage([Path] Guid applicationId, [Path] int sequenceNumber, [Path] int sectionNumber);

        [Post("/Moderator/Applications/{applicationId}/Sectors")]
        Task<List<ModeratorSector>> GetModeratorSectors([Path] Guid applicationId, [Body] GetModeratorSectorsRequest request); 

        [Get("/Moderator/Applications/{applicationId}/SectorDetails/{pageId}")]
        Task<SectorDetails> GetModeratorSectorDetails([Path] Guid applicationId, [Path] string pageId);

        [Get("/Moderator/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/BlindAssessmentOutcome")]
        Task<BlindAssessmentOutcome> GetBlindAssessmentOutcome([Path] Guid applicationId, [Path] int sequenceNumber, [Path] int sectionNumber, [Path] string pageId);

        [Post("/Moderator/Applications/{applicationId}/SubmitPageReviewOutcome")]
        Task<bool> SubmitModeratorPageReviewOutcome([Path] Guid applicationId, [Body]SubmitModeratorPageReviewOutcomeCommand command);

        [Post("/Moderator/Applications/{applicationId}/GetAllPageReviewOutcomes")]
        Task<List<ModeratorPageReviewOutcome>> GetAllModeratorPageReviewOutcomes([Path] Guid applicationId, [Body] GetAllModeratorPageReviewOutcomesRequest command);

        [Post("/Moderator/Applications/{applicationId}/GetPageReviewOutcome")] 
        Task<ModeratorPageReviewOutcome> GetModeratorPageReviewOutcome([Path] Guid applicationId, [Body] GetModeratorPageReviewOutcomeRequest command);

        [Post("/Moderator/Applications/{applicationId}/SubmitOutcome")]
        Task<bool> SubmitModerationOutcome([Path] Guid applicationId, [Body] SubmitOutcomeCommand command);
    }
}
