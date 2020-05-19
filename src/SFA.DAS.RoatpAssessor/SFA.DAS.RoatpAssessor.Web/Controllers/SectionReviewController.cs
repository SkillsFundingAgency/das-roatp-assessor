using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    [Authorize]
    public class SectionReviewController : RoatpAssessorControllerBase<SectionReviewController>
    {
        private readonly ISectionReviewOrchestrator _sectionReviewOrchestrator;
        public SectionReviewController(IRoatpApplicationApiClient applyApiClient,
                                       IRoatpAssessorPageValidator assessorPageValidator,
                                       ISectionReviewOrchestrator sectionReviewOrchestrator,
                                       ILogger<SectionReviewController> logger) : base(applyApiClient, logger, assessorPageValidator)
        {
            _sectionReviewOrchestrator = sectionReviewOrchestrator;
        }

        [HttpGet("SectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}")]
        [HttpGet("SectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}/Page/{pageId}")]
        public async Task<IActionResult> ReviewPageAnswers(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId)
        {
            var userId = HttpContext.User.UserId();

            if (sequenceNumber == 7 && sectionNumber == 6)
            {
                var sectorViewModel = await _sectionReviewOrchestrator.GetSectorsViewModel(new GetSectorsRequest(applicationId, userId));

                if (sectorViewModel is null)
                {
                    return RedirectToAction("ViewApplication", "Overview", new { applicationId });
                }

                return View("~/Views/SectionReview/ReviewSectors.cshtml", sectorViewModel);

            }

            var viewModel = await _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(applicationId, userId, sequenceNumber, sectionNumber, pageId, null));

            if (viewModel is null)
            {
                return RedirectToAction("ViewApplication", "Overview", new {applicationId});
            }

            return View("~/Views/SectionReview/ReviewAnswers.cshtml", viewModel);
        }



        [HttpGet]
        public async Task<IActionResult> ReviewSectorAnswers(Guid applicationId, string pageId, string title)
        {
            var userId = HttpContext.User.UserId();
            userId = "temp"; //TODO: Can't access the user until staff idams is enabled

            var viewModel = new SectorViewModel {ApplicationId = applicationId, PageId = pageId, Title = title};
            return View("~/Views/SectionReview/ReviewSectorAnswers.cshtml", viewModel);
        }

        [HttpPost("SectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}")]
        [HttpPost("SectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}/Page/{pageId}")]
        public async Task<IActionResult> ReviewPageAnswers(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, SubmitAssessorPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();

            Func<Task<ReviewAnswersViewModel>> viewModelBuilder = () => _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(command.ApplicationId, userId, command.SequenceNumber, command.SectionNumber, command.PageId, command.NextPageId));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"~/Views/SectionReview/ReviewAnswers.cshtml");
        }
    }
}