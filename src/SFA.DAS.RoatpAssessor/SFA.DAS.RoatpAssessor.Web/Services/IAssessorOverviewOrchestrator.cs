using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IAssessorOverviewOrchestrator
    {
        Task<AssessorApplicationViewModel> GetOverviewViewModel(GetApplicationOverviewRequest request);
        string GetSectionStatus(List<PageReviewOutcome> sectionPageReviewOutcomes, bool sectorSection);
    }
}
