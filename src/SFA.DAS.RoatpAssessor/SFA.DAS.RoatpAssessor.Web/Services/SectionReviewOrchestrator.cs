using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Enums;
using SFA.DAS.RoatpAssessor.Web.Helpers;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class SectionReviewOrchestrator : ISectionReviewOrchestrator
    {
        private readonly ILogger<SectionReviewOrchestrator> _logger;
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly ISupplementaryInformationService _supplementaryInformationService;

        public SectionReviewOrchestrator(ILogger<SectionReviewOrchestrator> logger, IRoatpApplicationApiClient applyApiClient, ISupplementaryInformationService supplementaryInformationService)
        {
            _logger = logger;
            _applyApiClient = applyApiClient;
            _supplementaryInformationService = supplementaryInformationService;
        }

        public async Task<ReviewAnswersViewModel> GetReviewAnswersViewModel(GetReviewAnswersRequest request)
        {
            var application = await _applyApiClient.GetApplication(request.ApplicationId);
            var assessorPage = await _applyApiClient.GetAssessorPage(request.ApplicationId, request.SequenceNumber, request.SectionNumber, request.PageId);

            if (application is null || assessorPage is null)
            {
                return null;
            }

            var viewModel = new ReviewAnswersViewModel
            {
                ApplicationId = application.ApplicationId,

                Ukprn = application.ApplyData.ApplyDetails.UKPRN,
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName,
                ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber,
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName,
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn,

                SequenceNumber = assessorPage.SequenceNumber,
                SectionNumber = assessorPage.SectionNumber,
                PageId = assessorPage.PageId,
                NextPageId = assessorPage.NextPageId,

                Caption = assessorPage.Caption,
                Heading = assessorPage.Heading,

                GuidanceText = !string.IsNullOrEmpty(assessorPage.BodyText) ? assessorPage.BodyText : assessorPage.Questions.FirstOrDefault()?.QuestionBodyText,

                Questions = new List<AssessorQuestion>(assessorPage.Questions),
                Answers = new List<AssessorAnswer>(assessorPage.Answers),
                TabularData = GetTabularDataFromQuestionsAndAnswers(assessorPage.Questions, assessorPage.Answers),
                SupplementaryInformation = await _supplementaryInformationService.GetSupplementaryInformation(application.ApplicationId, assessorPage.PageId)
            };

            //TODO: Can't access the user until staff idams is enabled
            viewModel.AssessorType = AssessorReviewHelpers.SetAssessorType(application, request.UserId); //  AssessorType.FirstAssessor; 

            //TODO: Explain why all of this is required? We're getting the viewmodel but I'm seeing stuff being submitted
            if (string.IsNullOrEmpty(request.PageId)) 
            {
                await ProcessAllAssessorPagesPerSection(request, viewModel);
            }

            await SetPageReviewOutcome(request, viewModel);

            return viewModel;
        }

        private List<TabularData> GetTabularDataFromQuestionsAndAnswers(List<AssessorQuestion> questions, List<AssessorAnswer> answers)
        {
            var tabularData = new List<TabularData>();

            if (questions != null && answers != null)
            {
                foreach (var tabularQuestion in questions.Where(q => QuestionInputType.TabularData.Equals(q.InputType, StringComparison.OrdinalIgnoreCase)))
                {
                    var jsonAnswer = answers.FirstOrDefault(a => a.QuestionId == tabularQuestion.QuestionId)?.Value;

                    if (jsonAnswer != null)
                    {
                        try
                        {
                            tabularData.Add(JsonConvert.DeserializeObject<TabularData>(jsonAnswer));
                        }
                        catch
                        {
                            // safe to ignore.
                            _logger.LogWarning($"Expected TabularData but was something else. Question Id: {tabularQuestion.QuestionId}");
                        }
                    }
                }
            }

            return tabularData;
        }

        private async Task ProcessAllAssessorPagesPerSection(GetReviewAnswersRequest request, ReviewAnswersViewModel viewModel)
        {
            var assessorReviewOutcomesPerSection = await _applyApiClient.GetAssessorReviewOutcomesPerSection(request.ApplicationId, request.SequenceNumber, request.SectionNumber, (int)viewModel.AssessorType, request.UserId);
            if (assessorReviewOutcomesPerSection is null || !assessorReviewOutcomesPerSection.Any())
            {
                // Start processing all subsequent pages and create record in AssessorPageReviewOutcome with emty status for each and every active page
                // Make a record for the first page
                await _applyApiClient.SubmitAssessorPageOutcome(request.ApplicationId,
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
                        await _applyApiClient.SubmitAssessorPageOutcome(request.ApplicationId,
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
            var assessorPage = await _applyApiClient.GetAssessorPage(applicationId, sequenceNumber, sectionNumber, pageId);           
            if (!string.IsNullOrEmpty(assessorPage.NextPageId))
            {
                return assessorPage.NextPageId;
            }

            return string.Empty;
        }

        private async Task SetPageReviewOutcome(GetReviewAnswersRequest request, ReviewAnswersViewModel viewModel)
        {
            // TODO: To think about... could we move this into Apply Service? It's really part of getting the assessor page back from the service
            var pageReviewOutcome = await _applyApiClient.GetPageReviewOutcome(request.ApplicationId, request.SequenceNumber, request.SectionNumber, viewModel.PageId, (int)viewModel.AssessorType, request.UserId);
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
