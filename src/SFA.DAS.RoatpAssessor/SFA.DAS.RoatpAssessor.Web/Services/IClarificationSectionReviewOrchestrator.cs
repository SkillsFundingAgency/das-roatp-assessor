using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IClarificationSectionReviewOrchestrator
    {
        Task<ClarifierReviewAnswersViewModel> GetReviewAnswersViewModel(GetReviewAnswersRequest request);

        Task<ApplicationSectorsViewModel> GetSectorsViewModel(GetSectorsRequest request);

        Task<ClarifierSectorDetailsViewModel> GetSectorDetailsViewModel(GetSectorDetailsRequest request);
    }
}
