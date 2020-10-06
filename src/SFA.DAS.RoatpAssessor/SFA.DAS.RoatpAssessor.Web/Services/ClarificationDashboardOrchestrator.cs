using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class ClarificationDashboardOrchestrator : IClarificationDashboardOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applicationApiClient;

        public ClarificationDashboardOrchestrator(IRoatpApplicationApiClient applicationApiClient)
        {
            _applicationApiClient = applicationApiClient;
        }

        public async Task<InClarificationApplicationsViewModel> GetInClarificationApplicationsViewModel(string userId)
        {
            var applicationSummary = await _applicationApiClient.GetApplicationCounts(userId);
            var applications = await _applicationApiClient.GetInClarificationApplications(userId);

            var viewModel = new InClarificationApplicationsViewModel(userId, applicationSummary.NewApplications, applicationSummary.InProgressApplications, applicationSummary.ModerationApplications, applicationSummary.ClarificationApplications);
            AddApplicationsToViewModel(viewModel, applications);
            return viewModel;
        }

        private void AddApplicationsToViewModel(InClarificationApplicationsViewModel viewModel, List<ClarificationApplicationSummary> applications)
        {
            foreach (var application in applications)
            {
                var applicationVm = CreateApplicationViewModel(application);
                viewModel.AddApplication(applicationVm);
            }
        }

        private ClarificationApplicationViewModel CreateApplicationViewModel(ClarificationApplicationSummary application)
        {
            var viewModel = new ClarificationApplicationViewModel();

            viewModel.ApplicationId = application.ApplicationId;
            viewModel.ApplicationReferenceNumber = application.ApplicationReferenceNumber;
            viewModel.ProviderRoute = application.ProviderRoute;
            viewModel.OrganisationName = application.OrganisationName;
            viewModel.Ukprn = application.Ukprn;
            viewModel.SubmittedDate = application.SubmittedDate;
            viewModel.ModeratorName = application.ModeratorName;
            viewModel.ClarificationRequestedDate = application.ClarificationRequestedDate;

            return viewModel;
        }
    }
}
