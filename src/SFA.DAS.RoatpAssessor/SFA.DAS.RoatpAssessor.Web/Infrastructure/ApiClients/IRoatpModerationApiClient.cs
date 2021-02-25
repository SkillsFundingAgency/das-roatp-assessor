using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpModerationApiClient
    {
        Task<List<ModeratorSequence>> GetModeratorSequences(Guid applicationId);

        Task<ModeratorPage> GetModeratorPage(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId);

        Task<List<ModeratorSector>> GetModeratorSectors(Guid applicationId, string userId);

        Task<SectorDetails> GetModeratorSectorDetails(Guid applicationId, string pageId);

        Task<BlindAssessmentOutcome> GetBlindAssessmentOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId);

        Task<bool> SubmitModeratorPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string userId, string userName, string status, string comment);

        Task<List<ModeratorPageReviewOutcome>> GetAllModeratorPageReviewOutcomes(Guid applicationId, string userId);
        Task<ModeratorPageReviewOutcome> GetModeratorPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string userId);
        Task<List<ModeratorPageReviewOutcome>> GetModeratorPageReviewOutcomesForSection(Guid applicationId, int sequenceNumber, int sectionNumber, string userId);
        Task<bool> SubmitModerationOutcome(Guid applicationId, string userId, string username, string status, string comment);
    }
}
