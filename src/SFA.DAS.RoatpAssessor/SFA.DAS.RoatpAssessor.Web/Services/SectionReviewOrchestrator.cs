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
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly ILogger<SectionReviewOrchestrator> _logger;

        public SectionReviewOrchestrator(IRoatpApplicationApiClient applyApiClient, ILogger<SectionReviewOrchestrator> logger)
        {

            _applyApiClient = applyApiClient;
            _logger = logger;
        }

        public async Task<ReviewAnswersViewModel> GetReviewAnswersViewModel(GetReviewAnswersRequest request)
        {
            var viewModel = new ReviewAnswersViewModel { ApplicationId = request.ApplicationId, SequenceNumber = request.SequenceNumber, SectionNumber = request.SectionNumber, PageId = request.PageId };


            var application = await _applyApiClient.GetApplication(request.ApplicationId);
            var assessorPage = await _applyApiClient.GetAssessorPage(request.ApplicationId, request.SequenceNumber, request.SectionNumber, request.PageId);

            viewModel = new ReviewAnswersViewModel
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
                
                GuidanceText = assessorPage.BodyText ?? assessorPage.Questions.FirstOrDefault()?.QuestionBodyText,

                Questions = new List<ApplyTypes.AssessorQuestion>(assessorPage.Questions),
                Answers = new List<ApplyTypes.AssessorAnswer>(assessorPage.Answers),
                TabularData = new List<TabularData>(),
                SupplementaryInformation = await GetSupplementaryInformation(application.ApplicationId, assessorPage.PageId)
            };

            foreach(var tabularQuestion in viewModel.Questions.Where(q => "TabularData".Equals(q.InputType, StringComparison.OrdinalIgnoreCase)))
            {
                var jsonAnswer = viewModel.Answers.FirstOrDefault(a => a.QuestionId == tabularQuestion.QuestionId)?.Value;

                if(jsonAnswer != null)
                {
                    viewModel.TabularData.Add(JsonConvert.DeserializeObject<TabularData>(jsonAnswer));
                }
            }

            var userId = "4dsfdg-MyGuidUserId-yf6re";
            viewModel.PageId = "TestPageId2";
            viewModel.NextPageId = "NextTestPageId2";
            viewModel.AssessorType = AssessorType.SecondAssessor; // SetAssessorType(application, userId);

            var assessorReviewOutcomesPerSection = await _applyApiClient.GetAssessorReviewOutcomesPerSection(request.ApplicationId, request.SequenceNumber, request.SectionNumber, (int)viewModel.AssessorType, userId);
            if (assessorReviewOutcomesPerSection != null && assessorReviewOutcomesPerSection.Any())
            {
                // GetTheFirstPage from QnA and map it to ViewModel
                // TODO:

                // Using the PageId => GetPageReviewOutcome
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
            }
            else
            {
                // Start processing all subsequent pages and create record in AssessorPageReviewOutcome with emty status for each and every active page
                // TODO:
                //await _applyApiClient.SubmitAssessorPageOutcome(request.ApplicationId,
                //                                    request.SequenceNumber,
                //                                    request.SectionNumber,
                //                                    PageIdFromTheProcessAbove,  
                //                                    (int)viewModel.AssessorType,
                //                                    userId,
                //                                    null,
                //                                    null);

                // Keep the data only for the first page, prepare the ViewModel for it and return it.
                // TODO:

            }



            // On 'Save and Continue' POST action => another method in SectionReviewOrchestrator
            // Get page data GetPage(ApplicationId, SequenceNumber, SectionNumber, NextPageId) {If NextPageId is null return to Application overview page; in Controller)
            // GetPageReviewStatus(ApplicationId, AssessorType, userId, PageId). We will need SequenceNumber & SectionNumber only if one page could apear in different sections. 
            // Prepare the ViewModel and return it


            return viewModel;
        }


        public async Task<ReviewAnswersViewModel> GetNextPageReviewAnswersViewModel(GetReviewAnswersRequest request)
        {
            var viewModel = new ReviewAnswersViewModel { ApplicationId = request.ApplicationId, SequenceNumber = request.SequenceNumber, SectionNumber = request.SectionNumber, PageId = request.PageId };


            var application = await _applyApiClient.GetApplication(request.ApplicationId);
            var assessorPage = await _applyApiClient.GetAssessorPage(request.ApplicationId, request.SequenceNumber, request.SectionNumber, request.PageId);

            viewModel = new ReviewAnswersViewModel
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

                GuidanceText = assessorPage.BodyText ?? assessorPage.Questions.FirstOrDefault()?.QuestionBodyText,

                Questions = new List<ApplyTypes.AssessorQuestion>(assessorPage.Questions),
                Answers = new List<ApplyTypes.AssessorAnswer>(assessorPage.Answers),
                TabularData = new List<TabularData>(),
                SupplementaryInformation = await GetSupplementaryInformation(application.ApplicationId, assessorPage.PageId)
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
            viewModel.PageId = "TestPageId2";
            viewModel.AssessorType = AssessorType.SecondAssessor; // SetAssessorType(application, userId);

            #region Don't need it 
            //var assessorReviewOutcomesPerSection = await _applyApiClient.GetAssessorReviewOutcomesPerSection(request.ApplicationId, request.SequenceNumber, request.SectionNumber, (int)viewModel.AssessorType, userId);
            //if (assessorReviewOutcomesPerSection != null && assessorReviewOutcomesPerSection.Any())
            //{
            //    // GetTheFirstPage from QnA and map it to ViewModel
            //    // TODO:

            //    // Using the PageId => GetPageReviewOutcome
            //    var pageReviewOutcome = await _applyApiClient.GetPageReviewOutcome(request.ApplicationId, request.SequenceNumber, request.SectionNumber, viewModel.PageId, (int)viewModel.AssessorType, userId);
            //    if (pageReviewOutcome != null)
            //    {
            //        viewModel.Status = pageReviewOutcome.Status;
            //        switch (pageReviewOutcome.Status)
            //        {
            //            case AssessorPageReviewStatus.Pass:
            //                viewModel.OptionPassText = pageReviewOutcome.Comment;
            //                break;
            //            case AssessorPageReviewStatus.Fail:
            //                viewModel.OptionFailText = pageReviewOutcome.Comment;
            //                break;
            //            case AssessorPageReviewStatus.InProgress:
            //                viewModel.OptionInProgressText = pageReviewOutcome.Comment;
            //                break;
            //            default:
            //                break;
            //        }
            //    }
            //}
            //else
            //{
            //    // Start processing all subsequent pages and create record in AssessorPageReviewOutcome with emty status for each and every active page
            //    // TODO:
            //    //await _applyApiClient.SubmitAssessorPageOutcome(request.ApplicationId,
            //    //                                    request.SequenceNumber,
            //    //                                    request.SectionNumber,
            //    //                                    PageIdFromTheProcessAbove,  
            //    //                                    (int)viewModel.AssessorType,
            //    //                                    userId,
            //    //                                    null,
            //    //                                    null);

            //    // Keep the data only for the first page, prepare the ViewModel for it and return it.
            //    // TODO:

            //}
            #endregion


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

        private async Task<List<AssessorSupplementaryInformation>> GetSupplementaryInformation(Guid applicationId, string pageId)
        {
            // NOTE: This is only required in one instance. If it is required more frequently then refactor to a service
            const string pageId_SafeguardingPolicyIncludesPreventDutyPolicy = "4037";

            List<AssessorSupplementaryInformation> supplementaryInformation = new List<AssessorSupplementaryInformation>();

            if (pageId == pageId_SafeguardingPolicyIncludesPreventDutyPolicy)
            {
                var safeGuardingPolicySupplementaryInformation = await GetSafeGuardingPolicySupplementaryInformation(applicationId);

                if(safeGuardingPolicySupplementaryInformation != null)
                {
                    supplementaryInformation.Add(safeGuardingPolicySupplementaryInformation);
                }
            }

            return supplementaryInformation;
        }

        private async Task<AssessorSupplementaryInformation> GetSafeGuardingPolicySupplementaryInformation(Guid applicationId)
        {
            const int sequenceNumber = 4;
            const int sectionNumber = 4;
            const string pageId = "4030";
            const string labelToUse = "Safeguarding policy";

            AssessorSupplementaryInformation supplementaryInformation = null;

            var page = await _applyApiClient.GetAssessorPage(applicationId, sequenceNumber, sectionNumber, pageId);

            if (page?.Questions?.First() != null)
            {
                var questionId = page.Questions.First().QuestionId;

                supplementaryInformation = new AssessorSupplementaryInformation
                {
                    ApplicationId = page.ApplicationId,
                    SequenceNumber = page.SequenceNumber,
                    SectionNumber = page.SectionNumber,
                    PageId = page.PageId,
                    Question = page.Questions.First(q => q.QuestionId == questionId),
                    Answer = page.Answers.First(a => a.QuestionId == questionId)
                };

                supplementaryInformation.Question.Label = labelToUse;
            }

            return supplementaryInformation;
        }
    }
}
