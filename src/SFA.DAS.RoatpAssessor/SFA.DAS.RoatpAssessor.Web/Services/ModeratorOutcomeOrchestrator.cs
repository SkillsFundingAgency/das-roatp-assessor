using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
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
            //STUBBED MFCMFC
            if (request.ApplicationId.ToString()== "f9d3a1ba-af26-465a-8e39-08d865e55e42")
            {
                
                ModeratorOutcomeViewModel viewModel = null;
                var apply = new Apply();
                apply.Id = Guid.NewGuid();
                apply.ApplicationId = request.ApplicationId;
                apply.OrganisationId = Guid.NewGuid();
                apply.ApplicationStatus = "GatewayAssessed";
                apply.ModerationStatus = "In Moderation";
                apply.ApplyData = new ApplyData
                {
                    ApplyDetails = new ApplyDetails
                    {
                        ReferenceNumber = "1233",
                        ProviderRoute = 1,
                        ProviderRouteName = "Main provider",
                        UKPRN = "12334",
                        OrganisationName = "Marky Marks Emporium",
                        ApplicationSubmittedOn = new DateTime(2020, 09, 30)
                    }
                };
                viewModel = new ModeratorOutcomeViewModel(apply, request.UserId);
                viewModel.PassCount = 50;
                viewModel.FailCount = 1;

                return viewModel;
            }

            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var contact = await _applicationApiClient.GetContactForApplication(request.ApplicationId);
            var sequences = await _moderationApiClient.GetModeratorSequences(request.ApplicationId);

            if (application is null || contact is null || sequences is null)
            {
                return null;
            }

            var viewmodel = new ModeratorOutcomeViewModel(application, request.UserId);

            var savedOutcomes = await _moderationApiClient.GetAllModeratorPageReviewOutcomes(request.ApplicationId, request.UserId);

            if (!savedOutcomes.Any()) return viewmodel;
         
            viewmodel.PassCount = savedOutcomes.Count(x => x.Status == ModeratorPageReviewStatus.Pass);
            viewmodel.FailCount = savedOutcomes.Count(x => x.Status == ModeratorPageReviewStatus.Fail);

            return viewmodel;
        }

        public async Task<ModeratorOutcomeReviewViewModel> GetInModerationOutcomeReviewViewModel(ReviewModeratorOutcomeRequest request)
        {
            var viewModel = new ModeratorOutcomeReviewViewModel
            {
                Status = request.Status, ReviewComment = request.ReviewComment
            };

            // STUBBED MFCMFC
            if (request.ApplicationId.ToString() == "f9d3a1ba-af26-465a-8e39-08d865e55e42")
            {
                var vm = new ModeratorOutcomeReviewViewModel();
                
                vm.ApplicantEmailAddress = "mark@test.com";
                vm.ApplicationRoute = "Main provider";
                vm.Ukprn = "12333444";
                vm.ApplyLegalName = "Marky Mark Emporium";
                vm.ReviewComment = request.ReviewComment;
                vm.Status = request.Status;

                return vm;
            }

            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            if (application is null)
            {
                return null;
            }

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