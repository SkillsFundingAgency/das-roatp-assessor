using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IClarificationOverviewOrchestrator
    {
        Task<ClarifierApplicationViewModel> GetOverviewViewModel(GetClarificationOverviewRequest request);
    }
}
