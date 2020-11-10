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
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Transformers;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class OutcomeSectionReviewOrchestrator : IOutcomeSectionReviewOrchestrator
    {
        private readonly ILogger<OutcomeSectionReviewOrchestrator> _logger;
        private readonly IRoatpApplicationApiClient _applicationApiClient;
        private readonly IRoatpModerationApiClient _moderationApiClient;
        private readonly IRoatpClarificationApiClient _clarificationApiClient;
        private readonly ISupplementaryInformationService _supplementaryInformationService;

        public OutcomeSectionReviewOrchestrator(ILogger<OutcomeSectionReviewOrchestrator> logger, IRoatpApplicationApiClient applicationApiClient, IRoatpModerationApiClient moderationApiClient,
            IRoatpClarificationApiClient clarificationApiClient, ISupplementaryInformationService supplementaryInformationService)
        {
            _logger = logger;
            _applicationApiClient = applicationApiClient;
            _moderationApiClient = moderationApiClient;
            _clarificationApiClient = clarificationApiClient;
            _supplementaryInformationService = supplementaryInformationService;
        }

        public async Task<OutcomeReviewAnswersViewModel> GetReviewAnswersViewModel(GetReviewAnswersRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var contact = await _applicationApiClient.GetContactForApplication(request.ApplicationId);
            var clarificationPage = await _clarificationApiClient.GetClarificationPage(request.ApplicationId, request.SequenceNumber, request.SectionNumber, request.PageId);

            if (application is null || contact is null || clarificationPage is null)
            {
                return null;
            }

            var viewModel = new OutcomeReviewAnswersViewModel
            {
                ApplicationId = application.ApplicationId,

                Ukprn = application.ApplyData.ApplyDetails.UKPRN,
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName,
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName,
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn,
                ApplicantEmailAddress = contact.Email,

                SequenceNumber = clarificationPage.SequenceNumber,
                SectionNumber = clarificationPage.SectionNumber,
                PageId = clarificationPage.PageId,
                NextPageId = clarificationPage.NextPageId,

                Caption = clarificationPage.Caption,
                Heading = clarificationPage.Heading,

                GuidanceInformation = clarificationPage.GuidanceInformation != null ? new List<string>(clarificationPage.GuidanceInformation) : new List<string>(),

                Questions = clarificationPage.Questions != null ? new List<Question>(clarificationPage.Questions) : new List<Question>(),
                Answers = clarificationPage.Answers != null ? new List<Answer>(clarificationPage.Answers) : new List<Answer>(),
                TabularData = GetTabularDataFromQuestionsAndAnswers(clarificationPage.Questions, clarificationPage.Answers),
                SupplementaryInformation = await _supplementaryInformationService.GetSupplementaryInformation(application.ApplicationId, clarificationPage.PageId),

                BlindAssessmentOutcome = await _moderationApiClient.GetBlindAssessmentOutcome(application.ApplicationId, clarificationPage.SequenceNumber, clarificationPage.SectionNumber, clarificationPage.PageId)
            };

            await SetPageReviewOutcome(request, viewModel);

            return viewModel;
        }

        public async Task<ApplicationSectorsViewModel> GetSectorsViewModel(GetSectorsRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);

            var clarificationPage = await _clarificationApiClient.GetClarificationPage(
                request.ApplicationId,
                SequenceIds.DeliveringApprenticeshipTraining,
                SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees,
                RoatpWorkflowPageIds.YourSectorsAndEmployeesStartingPageId);

            if (application is null || clarificationPage is null)
            {
                return null;
            }

            var selectedSectors = await _clarificationApiClient.GetClarificationSectors(request.ApplicationId, request.UserId);

            var viewModel = new ApplicationSectorsViewModel
            {
                ApplicationId = application.ApplicationId,
                Ukprn = application.ApplyData.ApplyDetails.UKPRN,
                ApplicantEmailAddress = null,
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName,
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName,
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn,
                Caption = clarificationPage.Caption,
                Heading = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployeesHeading,
                SelectedSectors = selectedSectors
            };

            return viewModel;
        }

        public async Task<OutcomeSectorDetailsViewModel> GetSectorDetailsViewModel(GetSectorDetailsRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var clarificationPage = await _clarificationApiClient.GetClarificationPage(
                request.ApplicationId,
                SequenceIds.DeliveringApprenticeshipTraining,
                SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees,
                RoatpWorkflowPageIds.YourSectorsAndEmployeesStartingPageId);

            if (application is null || clarificationPage is null)
            {
                return null;
            }

            var sectorDetails = await _clarificationApiClient.GetClarificationSectorDetails(request.ApplicationId, request.PageId);
            var blindAssessmentOutcome = await _moderationApiClient.GetBlindAssessmentOutcome(request.ApplicationId, SequenceIds.DeliveringApprenticeshipTraining, SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees, request.PageId);

            var viewModel = new OutcomeSectorDetailsViewModel
            {
                ApplicationId = application.ApplicationId,
                Ukprn = application.ApplyData.ApplyDetails.UKPRN,
                PageId = request.PageId,
                ApplicantEmailAddress = null,
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName,
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName,
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn,
                Caption = clarificationPage.Caption,
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

        private async Task SetSectorReviewOutcome(GetSectorDetailsRequest request, OutcomeSectorDetailsViewModel viewModel)
        {
            // TODO: To think about... could we move this into Apply Service? It's really part of getting the moderator page back from the service
            var pageReviewOutcome = await _clarificationApiClient.GetClarificationPageReviewOutcome(request.ApplicationId, SequenceIds.DeliveringApprenticeshipTraining,
                SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees, viewModel.PageId, request.UserId);

            if (pageReviewOutcome != null)
            {
                viewModel.Status = pageReviewOutcome.Status;
                switch (pageReviewOutcome.Status)
                {
                    case ClarificationPageReviewStatus.Pass:
                        viewModel.OptionPassText = pageReviewOutcome.Comment;
                        break;
                    case ClarificationPageReviewStatus.Fail:
                        viewModel.OptionFailText = pageReviewOutcome.Comment;
                        break;
                    case ClarificationPageReviewStatus.InProgress:
                        viewModel.OptionInProgressText = pageReviewOutcome.Comment;
                        break;
                    default:
                        break;
                }

                viewModel.ModerationOutcome = new ModerationOutcome
                {
                    ApplicationId = pageReviewOutcome.ApplicationId,
                    SequenceNumber = pageReviewOutcome.SequenceNumber,
                    SectionNumber = pageReviewOutcome.SectionNumber,
                    PageId = pageReviewOutcome.PageId,
                    ModeratorUserId = pageReviewOutcome.ModeratorUserId,
                    ModeratorUserName = pageReviewOutcome.ModeratorUserName,
                    ModeratorReviewStatus = pageReviewOutcome.ModeratorReviewStatus,
                    ModeratorReviewComment = pageReviewOutcome.ModeratorReviewComment
                };

                viewModel.ClarificationOutcome = new ApplyTypes.Outcome.ClarificationOutcome
                {
                    ApplicationId = pageReviewOutcome.ApplicationId,
                    SequenceNumber = pageReviewOutcome.SequenceNumber,
                    SectionNumber = pageReviewOutcome.SectionNumber,
                    PageId = pageReviewOutcome.PageId,
                    ClarifierUserId = pageReviewOutcome.UserId,
                    ClarifierUserName = pageReviewOutcome.UserName,
                    ClarificationReviewStatus = pageReviewOutcome.Status,
                    ClarificationReviewComment = pageReviewOutcome.Comment,
                    ClarificationResponse = pageReviewOutcome.ClarificationResponse,
                    ClarificationFiles = string.IsNullOrEmpty(pageReviewOutcome.ClarificationFile) ? null : new List<string> { pageReviewOutcome.ClarificationFile }
                };

                viewModel.ClarificationResponse = pageReviewOutcome.ClarificationResponse;
                viewModel.ClarificationFile = pageReviewOutcome.ClarificationFile;
            }
        }


        private async Task SetPageReviewOutcome(GetReviewAnswersRequest request, OutcomeReviewAnswersViewModel viewModel)
        {
            // TODO: To think about... could we move this into Apply Service? It's really part of getting the page back from the service
            var pageReviewOutcome = await _clarificationApiClient.GetClarificationPageReviewOutcome(request.ApplicationId, request.SequenceNumber, request.SectionNumber, viewModel.PageId, request.UserId);

            if (pageReviewOutcome != null)
            {
                viewModel.Status = pageReviewOutcome.Status;
                switch (pageReviewOutcome.Status)
                {
                    case ClarificationPageReviewStatus.Pass:
                        viewModel.OptionPassText = pageReviewOutcome.Comment;
                        break;
                    case ClarificationPageReviewStatus.Fail:
                        viewModel.OptionFailText = pageReviewOutcome.Comment;
                        break;
                    case ClarificationPageReviewStatus.InProgress:
                        viewModel.OptionInProgressText = pageReviewOutcome.Comment;
                        break;
                    default:
                        break;
                }

                viewModel.ModerationOutcome = new ModerationOutcome
                {
                    ApplicationId = pageReviewOutcome.ApplicationId,
                    SequenceNumber = pageReviewOutcome.SequenceNumber,
                    SectionNumber = pageReviewOutcome.SectionNumber,
                    PageId = pageReviewOutcome.PageId,
                    ModeratorUserId = pageReviewOutcome.ModeratorUserId,
                    ModeratorUserName = pageReviewOutcome.ModeratorUserName,
                    ModeratorReviewStatus = pageReviewOutcome.ModeratorReviewStatus,
                    ModeratorReviewComment = pageReviewOutcome.ModeratorReviewComment
                };

                viewModel.ClarificationOutcome = new ApplyTypes.Outcome.ClarificationOutcome
                {
                    ApplicationId = pageReviewOutcome.ApplicationId,
                    SequenceNumber = pageReviewOutcome.SequenceNumber,
                    SectionNumber = pageReviewOutcome.SectionNumber,
                    PageId = pageReviewOutcome.PageId,
                    ClarifierUserId = pageReviewOutcome.UserId,
                    ClarifierUserName = pageReviewOutcome.UserName,
                    ClarificationReviewStatus = pageReviewOutcome.Status,
                    ClarificationReviewComment = pageReviewOutcome.Comment,
                    ClarificationResponse = pageReviewOutcome.ClarificationResponse,
                    ClarificationFiles = string.IsNullOrEmpty(pageReviewOutcome.ClarificationFile) ? null : new List<string> { pageReviewOutcome.ClarificationFile }
                };

                viewModel.ClarificationResponse = pageReviewOutcome.ClarificationResponse;
                viewModel.ClarificationFile = pageReviewOutcome.ClarificationFile;
            }
        }
    }
}
