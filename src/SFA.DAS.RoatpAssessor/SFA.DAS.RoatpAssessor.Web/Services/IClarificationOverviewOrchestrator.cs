using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IClarificationOverviewOrchestrator
    {
        Task<ClarifierApplicationViewModel> GetOverviewViewModel(GetClarificationOverviewRequest request);
        string GetSectionStatus(List<ModeratorPageReviewOutcome> pageReviewOutcomes, int sequenceNumber, int sectionNumber);
        string GetSectorsSectionStatus(List<ModeratorPageReviewOutcome> pageReviewOutcomes);
    }
}
