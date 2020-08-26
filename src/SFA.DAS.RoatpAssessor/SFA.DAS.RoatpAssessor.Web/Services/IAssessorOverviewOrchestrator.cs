using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IAssessorOverviewOrchestrator
    {
        Task<AssessorApplicationViewModel> GetOverviewViewModel(GetAssessorOverviewRequest request);
        string GetSectionStatus(List<AssessorPageReviewOutcome> pageReviewOutcomes, int sequenceNumber, int sectionNumber);
        string GetSectorsSectionStatus(List<AssessorPageReviewOutcome> pageReviewOutcomes);
    }
}
