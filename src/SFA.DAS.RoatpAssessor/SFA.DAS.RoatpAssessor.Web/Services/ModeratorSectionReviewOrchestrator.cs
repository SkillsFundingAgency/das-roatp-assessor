using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Consts;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Enums;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Transformers;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class ModeratorSectionReviewOrchestrator : IModeratorSectionReviewOrchestrator
    {
        private readonly ILogger<ModeratorSectionReviewOrchestrator> _logger;
        private readonly IRoatpApplicationApiClient _applicationApiClient;
        private readonly IRoatpModerationApiClient _moderationApiClient;
        private readonly ISupplementaryInformationService _supplementaryInformationService;

        public ModeratorSectionReviewOrchestrator(ILogger<ModeratorSectionReviewOrchestrator> logger, IRoatpApplicationApiClient applicationApiClient, IRoatpModerationApiClient moderationApiClient, ISupplementaryInformationService supplementaryInformationService)
        {
            _logger = logger;
            _applicationApiClient = applicationApiClient;
            _moderationApiClient = moderationApiClient;
            _supplementaryInformationService = supplementaryInformationService;
        }

        public async Task<ModeratorReviewAnswersViewModel> GetReviewAnswersViewModel(GetReviewAnswersRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var contact = await _applicationApiClient.GetContactForApplication(request.ApplicationId);
            var moderatorPage = await _moderationApiClient.GetModeratorPage(request.ApplicationId, request.SequenceNumber, request.SectionNumber, request.PageId);

            if (application is null || contact is null || moderatorPage is null)
            {
                return null;
            }

            var viewModel = new ModeratorReviewAnswersViewModel
            {
                ApplicationId = application.ApplicationId,

                Ukprn = application.ApplyData.ApplyDetails.UKPRN,
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName,
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName,
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn,
                ApplicantEmailAddress = contact.Email,

                SequenceNumber = moderatorPage.SequenceNumber,
                SectionNumber = moderatorPage.SectionNumber,
                PageId = moderatorPage.PageId,
                NextPageId = moderatorPage.NextPageId,

                Caption = moderatorPage.Caption,
                Heading = moderatorPage.Heading,

                GuidanceInformation = moderatorPage.GuidanceInformation != null ? new List<string>(moderatorPage.GuidanceInformation) : new List<string>(),

                Questions = moderatorPage.Questions != null ? new List<Question>(moderatorPage.Questions) : new List<Question>(),
                Answers = moderatorPage.Answers != null ? new List<Answer>(moderatorPage.Answers) : new List<Answer>(),
                TabularData = GetTabularDataFromQuestionsAndAnswers(moderatorPage.Questions, moderatorPage.Answers),
                SupplementaryInformation = await _supplementaryInformationService.GetSupplementaryInformation(application.ApplicationId, moderatorPage.PageId),

                BlindAssessmentOutcome = await _moderationApiClient.GetBlindAssessmentOutcome(application.ApplicationId, moderatorPage.SequenceNumber, moderatorPage.SectionNumber, moderatorPage.PageId)
            };

            await SetPageReviewOutcome(request, viewModel);

            return viewModel;
        }

        public async Task<ApplicationSectorsViewModel> GetSectorsViewModel(GetSectorsRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);

            var moderatorPage = await _moderationApiClient.GetModeratorPage(
                request.ApplicationId,
                SequenceIds.DeliveringApprenticeshipTraining,
                SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees,
                RoatpWorkflowPageIds.YourSectorsAndEmployeesStartingPageId);

            if (application is null || moderatorPage is null)
            {
                return null;
            }

            var selectedSectors = await _moderationApiClient.GetModeratorSectors(request.ApplicationId, request.UserId);

            var viewModel = new ApplicationSectorsViewModel
            {
                ApplicationId = application.ApplicationId,
                Ukprn = application.ApplyData.ApplyDetails.UKPRN,
                ApplicantEmailAddress = null,
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName,
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName,
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn,
                Caption = moderatorPage.Caption,
                Heading = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployeesHeading,
                SelectedSectors = selectedSectors
            };

            return viewModel;
        }

        public async Task<ModeratorSectorDetailsViewModel> GetSectorDetailsViewModel(GetSectorDetailsRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var moderatorPage = await _moderationApiClient.GetModeratorPage(
                request.ApplicationId,
                SequenceIds.DeliveringApprenticeshipTraining,
                SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees,
                RoatpWorkflowPageIds.YourSectorsAndEmployeesStartingPageId);

            if (application is null || moderatorPage is null)
            {
                return null;
            }

            var sectorDetails = await _moderationApiClient.GetModeratorSectorDetails(request.ApplicationId, request.PageId);
            var blindAssessmentOutcome = await _moderationApiClient.GetBlindAssessmentOutcome(request.ApplicationId, SequenceIds.DeliveringApprenticeshipTraining,
                SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees, request.PageId);

            var viewModel = new ModeratorSectorDetailsViewModel
            {
                ApplicationId = application.ApplicationId,
                Ukprn = application.ApplyData.ApplyDetails.UKPRN,
                PageId = request.PageId,
                ApplicantEmailAddress = null,
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName,
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName,
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn,
                Caption = moderatorPage.Caption,
                Heading = $"Delivering training in '{sectorDetails?.SectorName}' sector",
                SectorDetails = sectorDetails,
                BlindAssessmentOutcome = blindAssessmentOutcome
            };

            await SetSectorReviewOutcome(request, viewModel);
            return viewModel;
        }

        private List<TabularData> GetTabularDataFromQuestionsAndAnswers(IEnumerable<Question> questions, IEnumerable<Answer> answers)
        {
            var tabularDataList = new List<TabularData>();

            if (questions != null && answers != null)
            {
                foreach (var tabularQuestion in questions.Where(q => QuestionInputType.TabularData.Equals(q.InputType, StringComparison.OrdinalIgnoreCase)))
                {
                    var questionId = tabularQuestion.QuestionId;
                    var jsonAnswer = answers.FirstOrDefault(a => a.QuestionId == questionId)?.Value;

                    if (jsonAnswer != null)
                    {
                        try
                        {
                            var tabularData = JsonConvert.DeserializeObject<TabularData>(jsonAnswer);

                            if (questionId == RoatpWorkflowQuestionIds.ManagementHierarchy)
                            {
                                tabularData = ManagementHierarchyTransformer.Transform(tabularData);
                            }

                            tabularDataList.Add(tabularData);
                        }
                        catch
                        {
                            // safe to ignore.
                            _logger.LogWarning($"Expected TabularData but was something else. Question Id: {questionId}");
                        }
                    }
                }
            }

            return tabularDataList;
        }

        private async Task SetSectorReviewOutcome(GetSectorDetailsRequest request, ModeratorSectorDetailsViewModel viewModel)
        {
            // TODO: To think about... could we move this into Apply Service? It's really part of getting the moderator page back from the service
            var pageReviewOutcome = await _moderationApiClient.GetModeratorPageReviewOutcome(request.ApplicationId, SequenceIds.DeliveringApprenticeshipTraining,
                SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees, viewModel.PageId, request.UserId);

            if (pageReviewOutcome != null)
            {
                viewModel.Status = pageReviewOutcome.Status;
                switch (pageReviewOutcome.Status)
                {
                    case ModeratorPageReviewStatus.Pass:
                        viewModel.OptionPassText = pageReviewOutcome.Comment;
                        break;
                    case ModeratorPageReviewStatus.Fail:
                        viewModel.OptionFailText = pageReviewOutcome.Comment;
                        break;
                    case ModeratorPageReviewStatus.InProgress:
                        viewModel.OptionInProgressText = pageReviewOutcome.Comment;
                        break;
                    default:
                        break;
                }
            }
        }


        private async Task SetPageReviewOutcome(GetReviewAnswersRequest request, ModeratorReviewAnswersViewModel viewModel)
        {
            // TODO: To think about... could we move this into Apply Service? It's really part of getting the moderator page back from the service
            var pageReviewOutcome = await _moderationApiClient.GetModeratorPageReviewOutcome(request.ApplicationId, request.SequenceNumber, request.SectionNumber, viewModel.PageId, request.UserId);

            if (pageReviewOutcome != null)
            {
                viewModel.Status = pageReviewOutcome.Status;
                switch (pageReviewOutcome.Status)
                {
                    case ModeratorPageReviewStatus.Pass:
                        viewModel.OptionPassText = pageReviewOutcome.Comment;
                        break;
                    case ModeratorPageReviewStatus.Fail:
                        viewModel.OptionFailText = pageReviewOutcome.Comment;
                        break;
                    case ModeratorPageReviewStatus.InProgress:
                        viewModel.OptionInProgressText = pageReviewOutcome.Comment;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
