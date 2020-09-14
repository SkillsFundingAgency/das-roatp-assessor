using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IAssessorSectionReviewOrchestrator
    {
        Task<AssessorReviewAnswersViewModel> GetReviewAnswersViewModel(GetReviewAnswersRequest request);

        Task<ApplicationSectorsViewModel> GetSectorsViewModel(GetSectorsRequest request);

        Task<AssessorSectorDetailsViewModel> GetSectorDetailsViewModel(GetSectorDetailsRequest request);
    }
}
