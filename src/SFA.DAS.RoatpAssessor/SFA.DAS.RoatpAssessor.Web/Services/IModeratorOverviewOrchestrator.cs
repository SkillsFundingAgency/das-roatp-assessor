using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IModeratorOverviewOrchestrator
    {
        Task<ModeratorApplicationViewModel> GetOverviewViewModel(GetModeratorOverviewRequest request);
        string GetSectionStatus(List<ModeratorPageReviewOutcome> sectionPageReviewOutcomes);
        string GetSectorsSectionStatus(IEnumerable<ModeratorSector> sectorsChosen, IEnumerable<ModeratorPageReviewOutcome> savedOutcomes);
    }
}
