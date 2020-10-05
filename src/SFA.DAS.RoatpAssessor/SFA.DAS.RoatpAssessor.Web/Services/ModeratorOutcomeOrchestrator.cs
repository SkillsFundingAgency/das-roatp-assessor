using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class ModeratorOutcomeOrchestrator : IModeratorOutcomeOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applicationApiClient;
        private readonly IRoatpModerationApiClient _moderationApiClient;

        public ModeratorOutcomeOrchestrator(IRoatpApplicationApiClient applicationApiClient, IRoatpModerationApiClient moderationApiClient)
        {
            _applicationApiClient = applicationApiClient;
            _moderationApiClient = moderationApiClient;
        }

        public async Task<ModeratorOutcomeViewModel> GetInModerationOutcomeViewModel(GetModeratorOutcomeRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var contact = await _applicationApiClient.GetContactForApplication(request.ApplicationId);
            var sequences = await _moderationApiClient.GetModeratorSequences(request.ApplicationId);

            if (application is null || contact is null || sequences is null)
            {
                return null;
            }

            var viewmodel = new ModeratorOutcomeViewModel(application, request.UserId);

            viewmodel.PassCount = 87;
            viewmodel.FailCount = 0;

            return viewmodel;
        }

    }
}