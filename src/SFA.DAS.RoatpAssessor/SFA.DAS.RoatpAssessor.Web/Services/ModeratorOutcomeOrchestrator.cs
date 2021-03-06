﻿using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
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
            var contact = await _applicationApiClient.GetContactForApplication(request.ApplicationId); // TODO: Is this needed?
            var savedOutcomes = await _moderationApiClient.GetAllModeratorPageReviewOutcomes(request.ApplicationId, request.UserId);

            if (application is null || contact is null || savedOutcomes is null)
            {
                return null;
            }

            // Not sure if this check is required. You're going to have Clarification ones in here soon
            var unsetOutcomesCount = savedOutcomes.Count(x =>
                x.Status != ModeratorPageReviewStatus.Pass && x.Status != ModeratorPageReviewStatus.Fail);

            if (unsetOutcomesCount > 0)
            {
                return null;
            }

            return new ModeratorOutcomeViewModel(application, savedOutcomes);
        }

        public async Task<ModeratorOutcomeReviewViewModel> GetInModerationOutcomeReviewViewModel(ReviewModeratorOutcomeRequest request)
        {
            var viewModel = new ModeratorOutcomeReviewViewModel
            {
                Status = request.Status, ReviewComment = request.ReviewComment, ApplicationId =  request.ApplicationId
            };

            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var contact = await _applicationApiClient.GetContactForApplication(request.ApplicationId);

            if (application is null || contact is null)
            {
                return null;
            }

            viewModel.ApplicantEmailAddress = contact.Email;

            if (application.ApplyData?.ApplyDetails != null)
            {
                viewModel.ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
                viewModel.Ukprn = application.ApplyData.ApplyDetails.UKPRN;
                viewModel.ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName;
                viewModel.SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
            }

            return viewModel;

        }
    }
}