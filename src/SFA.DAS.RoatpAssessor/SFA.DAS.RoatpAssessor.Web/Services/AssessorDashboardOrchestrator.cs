using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class AssessorDashboardOrchestrator : IAssessorDashboardOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applicationApiClient;
        private readonly IRoatpAssessorApiClient _assessorApiClient;

        public AssessorDashboardOrchestrator(IRoatpApplicationApiClient applicationApiClient, IRoatpAssessorApiClient assessorApiClient)
        {
            _applicationApiClient = applicationApiClient;
            _assessorApiClient = assessorApiClient;
        }

        public async Task<NewApplicationsViewModel> GetNewApplicationsViewModel(string userId)
        {
            var applicationSummary = await _applicationApiClient.GetApplicationCounts(userId);
            var applications = await _applicationApiClient.GetNewApplications(userId);

            var viewModel = new NewApplicationsViewModel(applicationSummary.NewApplications, applicationSummary.InProgressApplications, applicationSummary.ModerationApplications, applicationSummary.ClarificationApplications, applicationSummary.ClosedApplications);
            AddApplicationsToViewModel(viewModel, applications);
            return viewModel;
        }

        public async Task<bool> AssignApplicationToAssessor(Guid applicationId, int assessorNumber, string assessorUserId, string assessorName)
        {
            return await _assessorApiClient.AssignAssessor(applicationId, new AssignAssessorCommand(assessorNumber, assessorUserId, assessorName));
        }

        public async Task<InProgressApplicationsViewModel> GetInProgressApplicationsViewModel(string userId)
        {
            var applicationSummary = await _applicationApiClient.GetApplicationCounts(userId);
            var applications = await _applicationApiClient.GetInProgressApplications(userId);

            var viewModel = new InProgressApplicationsViewModel(userId, applicationSummary.NewApplications, applicationSummary.InProgressApplications, applicationSummary.ModerationApplications, applicationSummary.ClarificationApplications, applicationSummary.ClosedApplications);
            AddApplicationsToViewModel(viewModel, applications);
            return viewModel;
        }

        private void AddApplicationsToViewModel(AssessorDashboardViewModel viewModel, List<AssessorApplicationSummary> applications)
        {
            // TODO: Consider using a Mapper with unit tests, or just use the domain class instead
            foreach (var application in applications)
            {
                var applicationVm = CreateApplicationViewModel(application);

                viewModel.AddApplication(applicationVm);
            }
        }

        private ApplicationViewModel CreateApplicationViewModel(AssessorApplicationSummary application)
        {
            var viewModel = new ApplicationViewModel();

            viewModel.ApplicationId = application.ApplicationId;
            viewModel.ApplicationReferenceNumber = application.ApplicationReferenceNumber;
            viewModel.Assessor1Name = application.Assessor1Name;
            viewModel.Assessor2Name = application.Assessor2Name;
            viewModel.ProviderRoute = application.ProviderRoute;
            viewModel.OrganisationName = application.OrganisationName;
            viewModel.Ukprn = application.Ukprn;
            viewModel.SubmittedDate = application.SubmittedDate;
            viewModel.ApplicationStatus = application.ApplicationStatus;
            viewModel.Assessor1UserId = application.Assessor1UserId;
            viewModel.Assessor2UserId = application.Assessor2UserId;

            return viewModel;
        }
    }
}
