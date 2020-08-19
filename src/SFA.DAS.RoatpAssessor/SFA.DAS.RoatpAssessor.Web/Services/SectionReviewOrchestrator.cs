using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Consts;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Enums;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Helpers;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Transformers;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class SectionReviewOrchestrator : ISectionReviewOrchestrator
    {
        private readonly ILogger<SectionReviewOrchestrator> _logger;
        private readonly IRoatpApplicationApiClient _applicationApiClient;
        private readonly IRoatpAssessorApiClient _assessorApiClient;
        private readonly ISupplementaryInformationService _supplementaryInformationService;

        public SectionReviewOrchestrator(ILogger<SectionReviewOrchestrator> logger, IRoatpApplicationApiClient applicationApiClient, IRoatpAssessorApiClient assessorApiClient, ISupplementaryInformationService supplementaryInformationService)
        {
            _logger = logger;
            _applicationApiClient = applicationApiClient;
            _assessorApiClient = assessorApiClient;
            _supplementaryInformationService = supplementaryInformationService;
        }

        public async Task<ReviewAnswersViewModel> GetReviewAnswersViewModel(GetReviewAnswersRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var contact = await _applicationApiClient.GetContactForApplication(request.ApplicationId);
            var assessorPage = await _assessorApiClient.GetAssessorPage(request.ApplicationId, request.SequenceNumber, request.SectionNumber, request.PageId);

            if (application is null || contact is null || assessorPage is null)
            {
                return null;
            }

            var viewModel = new ReviewAnswersViewModel
            {
                ApplicationId = application.ApplicationId,
                AssessorType = AssessorReviewHelper.SetAssessorType(application, request.UserId),

                Ukprn = application.ApplyData.ApplyDetails.UKPRN,
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName,
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName,
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn,
                ApplicantEmailAddress = contact.Email,

                SequenceNumber = assessorPage.SequenceNumber,
                SectionNumber = assessorPage.SectionNumber,
                PageId = assessorPage.PageId,
                NextPageId = assessorPage.NextPageId,

                Caption = assessorPage.Caption,
                Heading = assessorPage.Heading,

                GuidanceInformation = assessorPage.GuidanceInformation,

                Questions = assessorPage.Questions != null ? new List<AssessorQuestion>(assessorPage.Questions) : new List<AssessorQuestion>(),
                Answers = assessorPage.Answers != null ? new List<AssessorAnswer>(assessorPage.Answers) : new List<AssessorAnswer>(),
                TabularData = GetTabularDataFromQuestionsAndAnswers(assessorPage.Questions, assessorPage.Answers),
                SupplementaryInformation = await _supplementaryInformationService.GetSupplementaryInformation(application.ApplicationId, assessorPage.PageId)
            };

            //TODO: Explain why all of this is required? We're getting the viewmodel but I'm seeing stuff being submitted
            if (string.IsNullOrEmpty(request.PageId))
            {
                await ProcessAllAssessorPagesPerSection(request, viewModel);
            }

            await SetPageReviewOutcome(request, viewModel);

            return viewModel;
        }

        public async Task<ApplicationSectorsViewModel> GetSectorsViewModel(GetSectorsRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var assessorPage = await _assessorApiClient.GetAssessorPage(
                request.ApplicationId,
                SequenceIds.DeliveringApprenticeshipTraining,
                SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees,
                RoatpWorkflowPageIds.YourSectorsAndEmployeesStartingPageId);

            if (application is null || assessorPage is null)
            {
                return null;
            }
            var viewModel = new ApplicationSectorsViewModel
            {
                ApplicationId = application.ApplicationId,
                Ukprn = application.ApplyData.ApplyDetails.UKPRN,
                ApplicantEmailAddress = null,
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName,
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName,
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn,
                Caption = assessorPage.Caption,
                Heading = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployeesHeading,
                SelectedSectors = await _assessorApiClient.GetChosenSectors(request.ApplicationId, request.UserId),
                GuidanceText = !string.IsNullOrEmpty(assessorPage.BodyText) ? assessorPage.BodyText : assessorPage.Questions?.FirstOrDefault()?.QuestionBodyText,
            };

            return viewModel;

        }

        public async Task<SectorViewModel> GetSectorViewModel(GetSectorDetailsRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var assessorPage = await _assessorApiClient.GetAssessorPage(
                request.ApplicationId,
                SequenceIds.DeliveringApprenticeshipTraining,
                SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees,
                RoatpWorkflowPageIds.YourSectorsAndEmployeesStartingPageId);

            if (application is null || assessorPage is null)
            {
                return null;
            }

            var sectorDetails = await _assessorApiClient.GetSectorDetails(request.ApplicationId, request.PageId);

            var viewModel = new SectorViewModel
            {
                ApplicationId = application.ApplicationId,
                Ukprn = application.ApplyData.ApplyDetails.UKPRN,
                AssessorType = AssessorReviewHelper.SetAssessorType(application, request.UserId),
                PageId = request.PageId,
                ApplicantEmailAddress = null,
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName,
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName,
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn,
                Caption = assessorPage.Caption,
                Heading = $"Delivering training in '{sectorDetails?.SectorName}' sector",
                SectorDetails = sectorDetails
            };


            await SetSectorReviewOutcome(request, viewModel);
            return viewModel;
        }

        private List<TabularData> GetTabularDataFromQuestionsAndAnswers(List<AssessorQuestion> questions, List<AssessorAnswer> answers)
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

        private async Task ProcessAllAssessorPagesPerSection(GetReviewAnswersRequest request, ReviewAnswersViewModel viewModel)
        {
            var assessorReviewOutcomesPerSection = await _assessorApiClient.GetAssessorReviewOutcomesPerSection(request.ApplicationId, request.SequenceNumber, request.SectionNumber, (int)viewModel.AssessorType, request.UserId);
            if (assessorReviewOutcomesPerSection is null || !assessorReviewOutcomesPerSection.Any())
            {
                // Start processing all subsequent pages and create record in AssessorPageReviewOutcome with emty status for each and every active page
                // Make a record for the first page
                await _assessorApiClient.SubmitAssessorPageOutcome(request.ApplicationId,
                                                    request.SequenceNumber,
                                                    request.SectionNumber,
                                                    viewModel.PageId,
                                                    (int)viewModel.AssessorType,
                                                    request.UserId,
                                                    null,
                                                    null);

                // TODO: Explain why you're checking NextPageId in the viewmodel and submitting the outcome
                if (!string.IsNullOrEmpty(viewModel.NextPageId)) // We have multiple pages
                {
                    var nextPageId = viewModel.NextPageId;
                    while (!string.IsNullOrEmpty(nextPageId))
                    {
                        await _assessorApiClient.SubmitAssessorPageOutcome(request.ApplicationId,
                                                                           request.SequenceNumber,
                                                                           request.SectionNumber,
                                                                           nextPageId,
                                                                           (int)viewModel.AssessorType,
                                                                           request.UserId,
                                                                           null,
                                                                           null);

                        nextPageId = await GetNextPageId(request.ApplicationId, request.SequenceNumber, request.SectionNumber, nextPageId);
                    }
                }
            }
        }


        private async Task<string> GetNextPageId(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId)
        {
            var assessorPage = await _assessorApiClient.GetAssessorPage(applicationId, sequenceNumber, sectionNumber, pageId);
            if (!string.IsNullOrEmpty(assessorPage?.NextPageId))
            {
                return assessorPage.NextPageId;
            }

            return string.Empty;
        }

        private async Task SetSectorReviewOutcome(GetSectorDetailsRequest request, SectorViewModel viewModel)
        {
            var pageReviewOutcome = await _assessorApiClient.GetPageReviewOutcome(request.ApplicationId, SequenceIds.DeliveringApprenticeshipTraining,
                SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees,
                viewModel.PageId, (int)viewModel.AssessorType, request.UserId);
            if (pageReviewOutcome != null)
            {
                viewModel.Status = pageReviewOutcome.Status;
                switch (pageReviewOutcome.Status)
                {
                    case AssessorPageReviewStatus.Pass:
                        viewModel.OptionPassText = pageReviewOutcome.Comment;
                        break;
                    case AssessorPageReviewStatus.Fail:
                        viewModel.OptionFailText = pageReviewOutcome.Comment;
                        break;
                    case AssessorPageReviewStatus.InProgress:
                        viewModel.OptionInProgressText = pageReviewOutcome.Comment;
                        break;
                    default:
                        break;
                }
            }
        }


        private async Task SetPageReviewOutcome(GetReviewAnswersRequest request, ReviewAnswersViewModel viewModel)
        {
            // TODO: To think about... could we move this into Apply Service? It's really part of getting the assessor page back from the service
            var pageReviewOutcome = await _assessorApiClient.GetPageReviewOutcome(request.ApplicationId, request.SequenceNumber, request.SectionNumber, viewModel.PageId, (int)viewModel.AssessorType, request.UserId);
            if (pageReviewOutcome != null)
            {
                viewModel.Status = pageReviewOutcome.Status;
                switch (pageReviewOutcome.Status)
                {
                    case AssessorPageReviewStatus.Pass:
                        viewModel.OptionPassText = pageReviewOutcome.Comment;
                        break;
                    case AssessorPageReviewStatus.Fail:
                        viewModel.OptionFailText = pageReviewOutcome.Comment;
                        break;
                    case AssessorPageReviewStatus.InProgress:
                        viewModel.OptionInProgressText = pageReviewOutcome.Comment;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
