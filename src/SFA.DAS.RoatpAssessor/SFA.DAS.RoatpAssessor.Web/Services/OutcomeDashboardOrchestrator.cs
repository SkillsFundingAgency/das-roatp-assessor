using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Outcome;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class OutcomeDashboardOrchestrator : IOutcomeDashboardOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applicationApiClient;

        public OutcomeDashboardOrchestrator(IRoatpApplicationApiClient applicationApiClient)
        {
            _applicationApiClient = applicationApiClient;
        }

        public async Task<ClosedApplicationsViewModel> GetClosedApplicationsViewModel(string userId, string searchTerm, string sortColumn, string sortOrder)
        {
            var applicationSummary = await _applicationApiClient.GetApplicationCounts(userId, searchTerm);
            var applications = await _applicationApiClient.GetClosedApplications(userId, searchTerm, sortColumn, sortOrder);

            var viewModel = new ClosedApplicationsViewModel(userId, applicationSummary.NewApplications, applicationSummary.InProgressApplications, applicationSummary.ModerationApplications, applicationSummary.ClarificationApplications, applicationSummary.ClosedApplications)
            {
                SearchTerm = searchTerm,
                SortColumn = sortColumn,
                SortOrder = sortOrder
            };

            AddApplicationsToViewModel(viewModel, applications);
            return viewModel;
        }

        private void AddApplicationsToViewModel(ClosedApplicationsViewModel viewModel, List<ClosedApplicationSummary> applications)
        {
            foreach (var application in applications)
            {
                var applicationVm = CreateApplicationViewModel(application);
                viewModel.AddApplication(applicationVm);
            }
        }

        private ClosedApplicationViewModel CreateApplicationViewModel(ClosedApplicationSummary application)
        {
            var viewModel = new ClosedApplicationViewModel();

            viewModel.ApplicationId = application.ApplicationId;
            viewModel.ApplicationReferenceNumber = application.ApplicationReferenceNumber;
            viewModel.ProviderRoute = application.ProviderRoute;
            viewModel.OrganisationName = application.OrganisationName;
            viewModel.Ukprn = application.Ukprn;
            viewModel.SubmittedDate = application.SubmittedDate;
            viewModel.ApplicationStatus = application.ApplicationStatus;
            viewModel.ModerationStatus = application.ModerationStatus;
            viewModel.OutcomeMadeDate = application.OutcomeMadeDate;
            viewModel.OutcomeMadeBy = application.OutcomeMadeBy;

            return viewModel;
        }
    }
}
