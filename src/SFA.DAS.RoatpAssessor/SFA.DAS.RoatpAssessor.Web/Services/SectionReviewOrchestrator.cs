using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
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

                Questions = new List<ApplyTypes.AssessorQuestion>(assessorPage.Questions),
                Answers = new List<ApplyTypes.AssessorAnswer>(assessorPage.Answers),
                TabularData = new List<TabularData>(),
                SupplementaryInformation = await _supplementaryInformationService.GetSupplementaryInformation(application.ApplicationId, assessorPage.PageId)
            };

            foreach (var tabularQuestion in viewModel.Questions.Where(q => "TabularData".Equals(q.InputType, StringComparison.OrdinalIgnoreCase)))
            {
                var jsonAnswer = viewModel.Answers.FirstOrDefault(a => a.QuestionId == tabularQuestion.QuestionId)?.Value;

                if (jsonAnswer != null)
                {
                    viewModel.TabularData.Add(JsonConvert.DeserializeObject<TabularData>(jsonAnswer));
                }
            }

            var userId = "4dsfdg-MyGuidUserId-yf6re";
            viewModel.AssessorType = AssessorType.FirstAssessor; // SetAssessorType(application, userId);

            if (string.IsNullOrEmpty(request.PageId)) // It's first page from the section
            {
                var assessorReviewOutcomesPerSection = await _applyApiClient.GetAssessorReviewOutcomesPerSection(request.ApplicationId, request.SequenceNumber, request.SectionNumber, (int)viewModel.AssessorType, userId);
                if (assessorReviewOutcomesPerSection != null && assessorReviewOutcomesPerSection.Any())
                {
                    //// This can be refactored
                    //var pageReviewOutcome = await _applyApiClient.GetPageReviewOutcome(request.ApplicationId, request.SequenceNumber, request.SectionNumber, viewModel.PageId, (int)viewModel.AssessorType, userId);
                    //if (pageReviewOutcome != null)
                    //{
                    //    viewModel.Status = pageReviewOutcome.Status;
                    //    switch (pageReviewOutcome.Status)
                    //    {
                    //        case AssessorPageReviewStatus.Pass:
                    //            viewModel.OptionPassText = pageReviewOutcome.Comment;
                    //            break;
                    //        case AssessorPageReviewStatus.Fail:
                    //            viewModel.OptionFailText = pageReviewOutcome.Comment;
                    //            break;
                    //        case AssessorPageReviewStatus.InProgress:
                    //            viewModel.OptionInProgressText = pageReviewOutcome.Comment;
                    //            break;
                    //        default:
                    //            break;
                    //    }
                    //}
                }
                else
                {
                    // Start processing all subsequent pages and create record in AssessorPageReviewOutcome with emty status for each and every active page
                    // Make a record for the first page
                    await _applyApiClient.SubmitAssessorPageOutcome(request.ApplicationId,
                                                        request.SequenceNumber,
                                                        request.SectionNumber,
                                                        viewModel.PageId,
                                                        (int)viewModel.AssessorType,
                                                        userId,
                                                        null,
                                                        null);

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
                                                                               userId,
                                                                               null,
                                                                               null);

                            nextPageId = await CreateAssessorPageOutcomeRecord(request.ApplicationId, request.SequenceNumber, request.SectionNumber, nextPageId, (int)viewModel.AssessorType, userId);

                        }
                    }
                }
            }

            var pageReviewOutcome = await _applyApiClient.GetPageReviewOutcome(request.ApplicationId, request.SequenceNumber, request.SectionNumber, viewModel.PageId, (int)viewModel.AssessorType, userId);
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

            return viewModel;
        }

        public async Task<string> CreateAssessorPageOutcomeRecord(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, int assessorType, string userId)
        {
            var assessorPage = await _applyApiClient.GetAssessorPage(applicationId, sequenceNumber, sectionNumber, pageId);           
            if (!string.IsNullOrEmpty(assessorPage.NextPageId))
            {
                return assessorPage.NextPageId;
            }

            return string.Empty;
        }


        public async Task<ReviewAnswersViewModel> GetNextPageReviewAnswersViewModel(GetReviewAnswersRequest request)
        {
            // TODO: Can we remove this function and use GetReviewAnswersViewModel ?

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

                Questions = new List<ApplyTypes.AssessorQuestion>(assessorPage.Questions),
                Answers = new List<ApplyTypes.AssessorAnswer>(assessorPage.Answers),
                TabularData = new List<TabularData>(),
                SupplementaryInformation = await _supplementaryInformationService.GetSupplementaryInformation(application.ApplicationId, assessorPage.PageId)
            };

            foreach (var tabularQuestion in viewModel.Questions.Where(q => "TabularData".Equals(q.InputType, StringComparison.OrdinalIgnoreCase)))
            {
                var jsonAnswer = viewModel.Answers.FirstOrDefault(a => a.QuestionId == tabularQuestion.QuestionId)?.Value;

                if (jsonAnswer != null)
                {
                    viewModel.TabularData.Add(JsonConvert.DeserializeObject<TabularData>(jsonAnswer));
                }
            }

            var userId = "4dsfdg-MyGuidUserId-yf6re";
            viewModel.AssessorType = AssessorType.FirstAssessor; // SetAssessorType(application, userId);


            // On 'Save and Continue' POST action => another method in SectionReviewOrchestrator
            // Get page data GetPage(ApplicationId, SequenceNumber, SectionNumber, NextPageId) {If NextPageId is null return to Application overview page; in Controller)
            // TODO
            // GetPageReviewStatus(ApplicationId, AssessorType, userId, PageId). We will need SequenceNumber & SectionNumber only if one page could apear in different sections.
            var pageReviewOutcome = await _applyApiClient.GetPageReviewOutcome(request.ApplicationId, request.SequenceNumber, request.SectionNumber, viewModel.PageId, (int)viewModel.AssessorType, userId);
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
            // Prepare the ViewModel and return it


            return viewModel;
        }

        // We will need to add Assessor1UserId & Assessor2UserId to Apply object
        // and when we can get UserId, we shall be able to SetAssessorType
        //private AssessorType SetAssessorType(Apply application, Guid userId)
        //{
        //    if (userId.Equals(application.Assessor1UserId))
        //    {
        //        return AssessorType.FirstAssessor;
        //    }
        //    else if ((userId.Equals(application.Assessor2UserId)
        //    {
        //        return AssessorType.SecondAssessor;
        //    }
        //    else
        //    {
        //        return AssessorType.Undefined;
        //    }
        //}
    }
}
