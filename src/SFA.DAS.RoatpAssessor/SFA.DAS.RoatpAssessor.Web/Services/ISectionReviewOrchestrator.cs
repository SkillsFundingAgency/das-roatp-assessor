using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface ISectionReviewOrchestrator
    {
        Task<ReviewAnswersViewModel> GetReviewAnswersViewModel(GetReviewAnswersRequest request);
        Task<ApplicationSectorsViewModel> GetSectorsViewModel(GetSectorsRequest request);

        Task<SectorViewModel> GetSectorViewModel(GetSectorDetailsRequest request);
    }
}
