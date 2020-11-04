using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IOutcomeSectionReviewOrchestrator
    {
        Task<OutcomeReviewAnswersViewModel> GetReviewAnswersViewModel(GetReviewAnswersRequest request);

        Task<ApplicationSectorsViewModel> GetSectorsViewModel(GetSectorsRequest request);

        Task<OutcomeSectorDetailsViewModel> GetSectorDetailsViewModel(GetSectorDetailsRequest request);
    }
}
