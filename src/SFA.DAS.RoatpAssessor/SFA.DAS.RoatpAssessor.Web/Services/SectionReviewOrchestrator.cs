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
            viewModel.PageId = "TestPageId";
            viewModel.AssessorType = AssessorType.SecondAssessor; // SetAssessorType(application, userId);

            // GetStatusesForAssessorSection(ApplicationId, SequenceNumber, SectionNumber, AssesssorType, userId)
            // IF ANY() => Get the first one (We have to ensure that first page is retrieved first)
            // and obtain page data for it GetPage(ApplicationId, SequenceNumber, SectionNumber, PageId)
            // prepare the ViewModel and return it.
            // IF NOT ANY() 
            // Start processing all subsequent pages and create record in AssessorPageReviewOutcome with emty status for each and every active page 
            // Keep the data only for the first page, prepare the ViewModel for it and return it.

            var pageReviewOutcome = await _applyApiClient.GetPageReviewOutcome(request.ApplicationId, request.SequenceNumber, request.SectionNumber, viewModel.PageId, (int)viewModel.AssessorType, userId);
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

            //var pageReviewOutcome = new PageReviewOutcome();

            // On 'Save and Continue' POST action => another method in SectionReviewOrchestrator
            // Get page data GetPage(ApplicationId, SequenceNumber, SectionNumber, NextPageId) {If NextPageId is null return to Application overview page; in Controller)
            // GetPageReviewStatus(ApplicationId, AssessorType, userId, PageId). We will need SequenceNumber & SectionNumber only if one page could apear in different sections. 
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
