using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpModerationApiClient
    {
        [Get("/Moderator/Applications/{applicationId}/Overview")]
        Task<List<ModeratorSequence>> GetModeratorSequences(Guid applicationId);

        [Get("/Moderator/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}")]
        Task<ModeratorPage> GetModeratorPage(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId);

        [Get("/Moderator/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page")]
        Task<ModeratorPage> GetModeratorPage(Guid applicationId, int sequenceNumber, int sectionNumber);

        [Post("/Moderator/Applications/{applicationId}/Sectors")]
        Task<List<ModeratorSector>> GetModeratorSectors(Guid applicationId, [Body] GetModeratorSectorsRequest request);

        [Get("/Moderator/Applications/{applicationId}/SectorDetails/{pageId}")]
        Task<SectorDetails> GetModeratorSectorDetails(Guid applicationId, string pageId);

        [Get("/Moderator/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/BlindAssessmentOutcome")]
        Task<BlindAssessmentOutcome> GetBlindAssessmentOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId);

        [Post("/Moderator/Applications/{applicationId}/SubmitPageReviewOutcome")]
        Task<ApiResponse<object>> SubmitModeratorPageReviewOutcome(Guid applicationId, [Body] SubmitModeratorPageReviewOutcomeCommand command);

        [Post("/Moderator/Applications/{applicationId}/GetAllPageReviewOutcomes")]
        Task<List<ModeratorPageReviewOutcome>> GetAllModeratorPageReviewOutcomes(Guid applicationId, GetAllModeratorPageReviewOutcomesRequest request);

        [Post("/Moderator/Applications/{applicationId}/GetPageReviewOutcome")]
        Task<ModeratorPageReviewOutcome> GetModeratorPageReviewOutcome(Guid applicationId, GetModeratorPageReviewOutcomeRequest request);

        [Post("/Moderator/Applications/{applicationId}/SubmitOutcome")]
        Task<ApiResponse<object>> SubmitModerationOutcome(Guid applicationId, SubmitOutcomeCommand command);
    }
}
