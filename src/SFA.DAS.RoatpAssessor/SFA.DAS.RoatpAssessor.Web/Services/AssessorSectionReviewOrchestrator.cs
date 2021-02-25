using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Consts;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Enums;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Transformers;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class AssessorSectionReviewOrchestrator : IAssessorSectionReviewOrchestrator
    {
        private readonly ILogger<AssessorSectionReviewOrchestrator> _logger;
        private readonly IRoatpApplicationApiClient _applicationApiClient;
        private readonly IRoatpAssessorApiClient _assessorApiClient;
        private readonly ISupplementaryInformationService _supplementaryInformationService;

        public AssessorSectionReviewOrchestrator(ILogger<AssessorSectionReviewOrchestrator> logger, IRoatpApplicationApiClient applicationApiClient, IRoatpAssessorApiClient assessorApiClient, ISupplementaryInformationService supplementaryInformationService)
        {
            _logger = logger;
            _applicationApiClient = applicationApiClient;
            _assessorApiClient = assessorApiClient;
            _supplementaryInformationService = supplementaryInformationService;
        }

        public async Task<AssessorReviewAnswersViewModel> GetReviewAnswersViewModel(GetReviewAnswersRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var contact = await _applicationApiClient.GetContactForApplication(request.ApplicationId);
            var assessorPage = await _assessorApiClient.GetAssessorPage(request.ApplicationId, request.SequenceNumber, request.SectionNumber, request.PageId);

            if (application is null || contact is null || assessorPage is null)
            {
                return null;
            }

            var viewModel = new AssessorReviewAnswersViewModel
            {
                ApplicationId = application.ApplicationId,

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

                GuidanceInformation = assessorPage.GuidanceInformation != null ? new List<string>(assessorPage.GuidanceInformation) : new List<string>(),

                Questions = assessorPage.Questions != null ? new List<Question>(assessorPage.Questions) : new List<Question>(),
                Answers = assessorPage.Answers != null ? new List<Answer>(assessorPage.Answers) : new List<Answer>(),
                TabularData = GetTabularDataFromQuestionsAndAnswers(assessorPage.Questions, assessorPage.Answers),
                SupplementaryInformation = await _supplementaryInformationService.GetSupplementaryInformation(application.ApplicationId, assessorPage.PageId)
            };

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

            var selectedSectors = await _assessorApiClient.GetAssessorSectors(request.ApplicationId, request.UserId);

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
                SelectedSectors = selectedSectors
            };

            return viewModel;

        }

        public async Task<AssessorSectorDetailsViewModel> GetSectorDetailsViewModel(GetSectorDetailsRequest request)
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

            var sectorDetails = await _assessorApiClient.GetAssessorSectorDetails(request.ApplicationId, request.PageId);

            var viewModel = new AssessorSectorDetailsViewModel
            {
                ApplicationId = application.ApplicationId,
                Ukprn = application.ApplyData.ApplyDetails.UKPRN,
                PageId = request.PageId,
                ApplicantEmailAddress = null,
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName,
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName,
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn,
                Caption = assessorPage.Caption,
                Heading = $"Delivering training in '{sectorDetails?.SectorName}' sector",
                SectorDetails = sectorDetails,
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

        private async Task SetSectorReviewOutcome(GetSectorDetailsRequest request, AssessorSectorDetailsViewModel viewModel)
        {
            // TODO: To think about... could we move this into Apply Service? It's really part of getting the assessor page back from the service
            var pageReviewOutcome = await _assessorApiClient.GetAssessorPageReviewOutcome(request.ApplicationId, SequenceIds.DeliveringApprenticeshipTraining,
                SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees, viewModel.PageId, request.UserId);

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


        private async Task SetPageReviewOutcome(GetReviewAnswersRequest request, AssessorReviewAnswersViewModel viewModel)
        {
            // TODO: To think about... could we move this into Apply Service? It's really part of getting the assessor page back from the service
            var pageReviewOutcome = await _assessorApiClient.GetAssessorPageReviewOutcome(request.ApplicationId, request.SequenceNumber, request.SectionNumber, viewModel.PageId, request.UserId);

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
