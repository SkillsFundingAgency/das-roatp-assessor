﻿using Microsoft.Extensions.Logging;
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

            if(application is null || assessorPage is null)
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

            foreach(var tabularQuestion in viewModel.Questions.Where(q => "TabularData".Equals(q.InputType, StringComparison.OrdinalIgnoreCase)))
            {
                var jsonAnswer = viewModel.Answers.FirstOrDefault(a => a.QuestionId == tabularQuestion.QuestionId)?.Value;

                if(jsonAnswer != null)
                {
                    viewModel.TabularData.Add(JsonConvert.DeserializeObject<TabularData>(jsonAnswer));
                }
            }

            var userId = "4dsfdg-MyGuidUserId-yf6re";
            //viewModel.PageId = "TestPageId2";
            //viewModel.NextPageId = "NextTestPageId2";
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
            //viewModel.PageId = "TestPageId2";
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
    }
}
