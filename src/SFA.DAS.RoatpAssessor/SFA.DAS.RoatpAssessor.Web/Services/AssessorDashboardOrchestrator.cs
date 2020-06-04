using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class AssessorDashboardOrchestrator : IAssessorDashboardOrchestrator
    {
        private readonly IRoatpAssessorApiClient _apiClient;

        public AssessorDashboardOrchestrator(IRoatpAssessorApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<NewApplicationsViewModel> GetNewApplicationsViewModel(string userId)
        {
            var applicationSummary = await _apiClient.GetAssessorSummary(userId);
            var applications = await _apiClient.GetNewApplications(userId);

            var viewModel = new NewApplicationsViewModel(applicationSummary.NewApplications, applicationSummary.InProgressApplications, applicationSummary.ModerationApplications, applicationSummary.ClarificationApplications);
            AddApplicationsToViewModel(viewModel, applications);
            return viewModel;
        }

        public async Task AssignApplicationToAssessor(Guid applicationId, int assessorNumber, string assessorUserId, string assessorName)
        {
            await _apiClient.AssignAssessor(applicationId, new AssignAssessorApplicationRequest(assessorNumber, assessorUserId, assessorName));
        }

        public async Task<InProgressApplicationsViewModel> GetInProgressApplicationsViewModel(string userId)
        {
            var applicationSummary = await _apiClient.GetAssessorSummary(userId);
            var applications = await _apiClient.GetInProgressApplications(userId);

            var viewModel = new InProgressApplicationsViewModel(userId, applicationSummary.NewApplications, applicationSummary.InProgressApplications, applicationSummary.ModerationApplications, applicationSummary.ClarificationApplications);
            AddApplicationsToViewModel(viewModel, applications);
            return viewModel;
        }

        private void AddApplicationsToViewModel(DashboardViewModel viewModel, List<RoatpAssessorApplicationSummary> applications)
        {
            // TODO: Consider using a Mapper with unit tests, or just use the domain class instead
            foreach (var application in applications)
            {
                var applicationVm = new ApplicationViewModel
                {
                    ApplicationId = application.ApplicationId,
                    ApplicationReferenceNumber = application.ApplicationReferenceNumber,
                    Assessor1Name = application.Assessor1Name,
                    Assessor2Name = application.Assessor2Name,
                    ProviderRoute = application.ProviderRoute,
                    OrganisationName = application.OrganisationName,
                    Ukprn = application.Ukprn,
                    SubmittedDate = application.SubmittedDate,
                    Assessor1UserId = application.Assessor1UserId,
                    Assessor2UserId = application.Assessor2UserId
                };

                viewModel.AddApplication(applicationVm);
            }
        }
    }
}
