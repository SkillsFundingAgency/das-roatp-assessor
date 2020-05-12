using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    public class SectionReviewController : RoatpAssessorControllerBase<SectionReviewController>
    {
        private readonly ISectionReviewOrchestrator _sectionReviewOrchestrator;
        public SectionReviewController(IRoatpApplicationApiClient applyApiClient,
                                        IHttpContextAccessor contextAccessor,
                                        IRoatpAssessorPageValidator assessorPageValidator,
                                         ISectionReviewOrchestrator sectionReviewOrchestrator,
                                        ILogger<SectionReviewController> logger) : base(contextAccessor, applyApiClient, logger, assessorPageValidator)
        {
            _sectionReviewOrchestrator = sectionReviewOrchestrator;
        }

        [HttpGet]
        public async Task<IActionResult> ReviewPageAnswers(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId)
        {
            var userName = User.UserDisplayName();
            var userId = "";

            // TODO: 1st check - does the application exist?
            // TODO: 2nd check - is sequence number within bounds? Shouldn't be showing sequence 1 for example
            // TODO: 3rd check - is it in the appropriate state for Assessor Review?
            // TODO: 4th check - should it be shown in read only mode or not?

            ReviewAnswersViewModel viewModel;

            if (string.IsNullOrWhiteSpace(pageId))
            {
                viewModel = await _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(applicationId, userId, sequenceNumber, sectionNumber, pageId, null));
            }
            else
            {
                // TODO: Could we remove this altogether and have one function to get the view model?
                viewModel = await _sectionReviewOrchestrator.GetNextPageReviewAnswersViewModel(new GetReviewAnswersRequest(applicationId, userId, sequenceNumber, sectionNumber, pageId, null));
            }

            if (viewModel is null)
            {
                return Redirect($"/Home/{applicationId}");
            }

            return View("~/Views/Home/ReviewAnswers.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ReviewPageAnswers(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, SubmitAssessorPageAnswerCommand command)
        {
            // NOTE: GET & POST should not modify URLs (i.e. should be on same URL)
            // Is it possible to do this? See RoatpFinancialController in Admin Service on how to do this!

            // TODO: 1st check - does the application exist?
            // TODO: 2nd check - is sequence number within bounds? Shouldn't be showing sequence 1 for example
            // TODO: 3rd check - is it in the appropriate state for Assessor Review?
            // TODO: 4th check - should it be shown in read only mode or not?

            var userId = User.UserDisplayName(); // TODO: to be changed to UserId
            Func<Task<ReviewAnswersViewModel>> viewModelBuilder = () => _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(command.ApplicationId, userId, command.SequenceNumber, command.SectionNumber, command.PageId, command.NextPageId));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"~/Views/Home/ReviewAnswers.cshtml");
        }
    }
}