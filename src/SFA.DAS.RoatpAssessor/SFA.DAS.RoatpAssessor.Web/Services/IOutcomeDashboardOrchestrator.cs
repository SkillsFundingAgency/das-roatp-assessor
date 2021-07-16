using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IOutcomeDashboardOrchestrator
    {
        Task<ClosedApplicationsViewModel> GetClosedApplicationsViewModel(string userId, string searchTerm, string sortColumn, string sortOrder);
    }
}
