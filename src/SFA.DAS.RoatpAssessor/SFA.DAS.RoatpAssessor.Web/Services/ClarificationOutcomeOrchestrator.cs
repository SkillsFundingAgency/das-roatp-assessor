using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class ClarificationOutcomeOrchestrator:IClarificationOutcomeOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applicationApiClient;
        private readonly IRoatpClarificationApiClient _clarificationApiClient;

        public ClarificationOutcomeOrchestrator(IRoatpApplicationApiClient applicationApiClient, IRoatpClarificationApiClient clarificationApiClient)
        {
            _applicationApiClient = applicationApiClient;
            _clarificationApiClient = clarificationApiClient;
        }

        public async Task<ClarificationOutcomeViewModel> GetClarificationOutcomeViewModel(GetClarificationOutcomeRequest request)
        {


            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var contact = await _applicationApiClient.GetContactForApplication(request.ApplicationId);
            var outcomes = await _clarificationApiClient.GetAllClarificationPageReviewOutcomes(request.ApplicationId, request.UserId);

            if (application is null || contact is null || outcomes is null)
            {
                return null;
            }

            var unsetOutcomesCount = outcomes.Count(x =>
                x.Status != ClarificationPageReviewStatus.Pass && x.Status != ClarificationPageReviewStatus.Fail);

            if (unsetOutcomesCount > 0)
            {
                return null;
            }

            return new ClarificationOutcomeViewModel(application, outcomes);

        }

        public async Task<ClarificationOutcomeReviewViewModel> GetClarificationOutcomeReviewViewModel(ReviewClarificationOutcomeRequest request)
        {
            var viewModel = new ClarificationOutcomeReviewViewModel
            {
                Status = request.Status,
                ReviewComment = request.ReviewComment,
                ApplicationId = request.ApplicationId
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

