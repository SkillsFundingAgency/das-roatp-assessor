using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

                Caption = assessorPage.SequenceTitle,
                Heading = assessorPage.Title,
                
                GuidanceText = assessorPage.BodyText ?? assessorPage.Questions[0].QuestionBodyText,

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


            return viewModel;
        }
    }
}
