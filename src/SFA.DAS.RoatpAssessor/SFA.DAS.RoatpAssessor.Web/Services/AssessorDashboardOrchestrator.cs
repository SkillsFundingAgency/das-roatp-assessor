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
            var applications = await _apiClient.GetNewApplications(userId);

            var viewModel = new NewApplicationsViewModel(0, 0, 0, 0);
            AddApplicationsToViewModel(viewModel, applications);
            return viewModel;
        }

        public async Task AssignApplicationToAssessor(Guid applicationId, int assessorNumber, string assessorUserId, string assessorName)
        {
            await _apiClient.AssignAssessor(applicationId, new AssignAssessorApplicationRequest(assessorNumber, assessorUserId, assessorName));
        }

        private void AddApplicationsToViewModel(NewApplicationsViewModel viewModel, List<RoatpAssessorApplicationSummary> applications)
        {
            foreach (var application in applications)
            {
                var applicationVm = new ApplicationViewModel
                {
                    ApplicationId = application.ApplicationId,
                    ApplicationReferenceNumber = application.ApplicationReferenceNumber,
                    Assessor1 = application.Assessor1Name,
                    Assessor2 = application.Assessor2Name,
                    ProviderRoute = application.ProviderRoute,
                    OrganisationName = application.OrganisationName,
                    Ukprn = application.Ukprn,
                    SubmittedDate = application.SubmittedDate
                };

                viewModel.AddApplication(applicationVm);
            }
        }
    }
}
