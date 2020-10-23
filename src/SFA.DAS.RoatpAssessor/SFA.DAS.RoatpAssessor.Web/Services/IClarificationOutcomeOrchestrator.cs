using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IClarificationOutcomeOrchestrator
    {
        Task<ClarificationOutcomeViewModel> GetClarificationOutcomeViewModel(GetClarificationOutcomeRequest request);
        Task<ClarificationOutcomeReviewViewModel> GetClarificationOutcomeReviewViewModel(ReviewClarificationOutcomeRequest request);
    }
}