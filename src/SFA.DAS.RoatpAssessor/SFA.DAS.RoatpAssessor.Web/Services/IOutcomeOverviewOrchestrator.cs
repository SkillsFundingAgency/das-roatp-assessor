using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IOutcomeOverviewOrchestrator
    {
        // MFCMFC change all moderator references to Outcome
        Task<OutcomeApplicationViewModel> GetOverviewViewModel(GetOutcomeOverviewRequest request);
        string GetSectionStatus(List<ModeratorPageReviewOutcome> pageReviewOutcomes, int sequenceNumber, int sectionNumber);
        string GetSectorsSectionStatus(List<ModeratorPageReviewOutcome> pageReviewOutcomes);
    }
}
