using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpModerationApiClient
    {
        Task<List<ModeratorSequence>> GetModeratorSequences(Guid applicationId);

        Task<List<ModeratorPageReviewOutcome>> GetAllModeratorPageReviewOutcomes(Guid applicationId, string userId);
        Task<ModeratorPageReviewOutcome> GetModeratorPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string userId);
        Task<List<ModeratorPageReviewOutcome>> GetModeratorPageReviewOutcomesForSection(Guid applicationId, int sequenceNumber, int sectionNumber, string userId);

        Task<List<ModeratorSector>> GetModeratorSectors(Guid applicationId, string userId);
    }
}
