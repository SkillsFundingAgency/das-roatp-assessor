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
        public async Task<IActionResult> ReviewPageAnswers(Guid applicationId, int sequenceNumber, int sectionNumber)
        {
            var userName = User.UserDisplayName();
            var userId = "";

            var viewModel = await _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(applicationId, userName, sequenceNumber, sectionNumber));
            viewModel.PageId = "TestPageId";

            if (viewModel is null)
            {
                return Redirect($"/Home/{applicationId}");
            }

            return View("~/Views/Home/ReviewAnswers.cshtml", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EvaluatePageAnswers(SubmitAssessorPageAnswerCommand command)
        {
            var userName = User.UserDisplayName();
            var userId = "";
            Func<Task<ReviewAnswersViewModel>> viewModelBuilder = () => _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(command.ApplicationId, userName, command.SequenceNumber, command.SectionNumber));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"~/Views/Home/ReviewAnswers.cshtml");
        }
    }
}