﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class AssessorDashboardOrchestrator : IAssessorDashboardOrchestrator
    {
        private readonly IRoatpAssessorApiClient _assessorApiClient;
        private readonly IRoatpModerationApiClient _roatpModerationApiClient;

        public AssessorDashboardOrchestrator(IRoatpAssessorApiClient assessorApiClient, IRoatpModerationApiClient roatpModerationApiClient)
        {
            _assessorApiClient = assessorApiClient;
            _roatpModerationApiClient = roatpModerationApiClient;
        }

        public async Task<NewApplicationsViewModel> GetNewApplicationsViewModel(string userId)
        {
            var applicationSummary = await _assessorApiClient.GetAssessorSummary(userId);
            var applications = await _assessorApiClient.GetNewApplications(userId);

            var viewModel = new NewApplicationsViewModel(applicationSummary.NewApplications, applicationSummary.InProgressApplications, applicationSummary.ModerationApplications, applicationSummary.ClarificationApplications);
            AddApplicationsToViewModel(viewModel, applications);
            return viewModel;
        }

        public async Task AssignApplicationToAssessor(Guid applicationId, int assessorNumber, string assessorUserId, string assessorName)
        {
            await _assessorApiClient.AssignAssessor(applicationId, new AssignAssessorApplicationRequest(assessorNumber, assessorUserId, assessorName));
        }

        public async Task<InProgressApplicationsViewModel> GetInProgressApplicationsViewModel(string userId)
        {
            var applicationSummary = await _assessorApiClient.GetAssessorSummary(userId);
            var applications = await _assessorApiClient.GetInProgressApplications(userId);

            var viewModel = new InProgressApplicationsViewModel(userId, applicationSummary.NewApplications, applicationSummary.InProgressApplications, applicationSummary.ModerationApplications, applicationSummary.ClarificationApplications);
            AddApplicationsToViewModel(viewModel, applications);
            return viewModel;
        }

        public async Task<InModerationApplicationsViewModel> GetInModerationApplicationsViewModel(string userId)
        {
            var applicationSummary = await _assessorApiClient.GetAssessorSummary(userId);
            var applications = await _roatpModerationApiClient.GetModerationApplications();

            var viewModel = new InModerationApplicationsViewModel(userId, applicationSummary.NewApplications, applicationSummary.InProgressApplications, applicationSummary.ModerationApplications, applicationSummary.ClarificationApplications);
            AddApplicationsToViewModel(viewModel, applications);
            return viewModel;
        }

        private void AddApplicationsToViewModel(AssessorDashboardViewModel viewModel, List<RoatpAssessorApplicationSummary> applications)
        {
            foreach (var application in applications)
            {
                var applicationVm = CreateApplicationViewModel<ApplicationViewModel>(application);

                viewModel.AddApplication(applicationVm);
            }
        }

        private void AddApplicationsToViewModel(InModerationApplicationsViewModel viewModel, List<RoatpModerationApplicationSummary> applications)
        {
            foreach (var application in applications)
            {
                var applicationVm = CreateApplicationViewModel<ModerationApplicationViewModel>(application);
                applicationVm.Status = application.Status;
                viewModel.AddApplication(applicationVm);
            }
        }

        private TViewModel CreateApplicationViewModel<TViewModel>(RoatpAssessorApplicationSummary application) where TViewModel : ApplicationViewModel, new()
        {
            var viewModel = new TViewModel();

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

            return viewModel;
        }
    }
}
