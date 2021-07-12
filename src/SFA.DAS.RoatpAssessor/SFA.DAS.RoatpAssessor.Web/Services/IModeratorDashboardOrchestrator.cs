using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IModeratorDashboardOrchestrator
    {
        Task<InModerationApplicationsViewModel> GetInModerationApplicationsViewModel(string userId, string sortOrder, string sortColumn);
    }
}
