using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IAssessorOverviewOrchestrator
    {
        Task<AssessorApplicationViewModel> GetOverviewViewModel(GetApplicationOverviewRequest request);
    }
}
