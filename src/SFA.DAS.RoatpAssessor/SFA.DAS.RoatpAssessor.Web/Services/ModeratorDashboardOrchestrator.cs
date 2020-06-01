using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class ModeratorDashboardOrchestrator : IModeratorDashboardOrchestrator
    {
        private readonly IRoatpAssessorApiClient _assessorApiClient;
        private readonly IRoatpModerationApiClient _roatpModerationApiClient;

        public ModeratorDashboardOrchestrator(IRoatpAssessorApiClient assessorApiClient, IRoatpModerationApiClient roatpModerationApiClient)
        {
            _assessorApiClient = assessorApiClient;
            _roatpModerationApiClient = roatpModerationApiClient;
        }

        public async Task<InModerationApplicationsViewModel> GetInModerationApplicationsViewModel(string userId)
        {
            var applicationSummary = await _assessorApiClient.GetAssessorSummary(userId);
            var applications = await _roatpModerationApiClient.GetModerationApplications();

            var viewModel = new InModerationApplicationsViewModel(userId, applicationSummary.NewApplications, applicationSummary.InProgressApplications, applicationSummary.ModerationApplications, applicationSummary.ClarificationApplications);
            AddApplicationsToViewModel(viewModel, applications);
            return viewModel;
        }

        private void AddApplicationsToViewModel(InModerationApplicationsViewModel viewModel, List<RoatpModerationApplicationSummary> applications)
        {
            foreach (var application in applications)
            {
                var applicationVm = CreateApplicationViewModel<ModerationApplicationViewModel>(application);
                viewModel.AddApplication(applicationVm);
            }
        }

        private ModerationApplicationViewModel CreateApplicationViewModel<TViewModel>(RoatpModerationApplicationSummary application)
        {
            var viewModel = new ModerationApplicationViewModel();

            viewModel.ApplicationId = application.ApplicationId;
            viewModel.ApplicationReferenceNumber = application.ApplicationReferenceNumber;
            viewModel.Assessor1Name = application.Assessor1Name;
            viewModel.Assessor2Name = application.Assessor2Name;
            viewModel.ProviderRoute = application.ProviderRoute;
            viewModel.OrganisationName = application.OrganisationName;
            viewModel.Ukprn = application.Ukprn;
            viewModel.SubmittedDate = application.SubmittedDate;
            viewModel.Assessor1UserId = application.Assessor1UserId;
            viewModel.Assessor2UserId = application.Assessor2UserId;
            viewModel.Status = application.Status;

            return viewModel;
        }
    }
}
