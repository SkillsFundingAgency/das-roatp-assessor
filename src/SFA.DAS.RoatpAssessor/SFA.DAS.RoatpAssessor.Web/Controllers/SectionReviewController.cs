using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using SFA.DAS.RoatpAssessor.Web.Domain;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
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

        
            if (sequenceNumber == SequenceIds.DeliveringApprenticeshipTraining && sectionNumber == SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees)
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



        [HttpGet("SectionReview/{applicationId}/Sector/{PageId}")]
        public async Task<IActionResult> ReviewSectorAnswers(Guid applicationId, string pageId)
        {
            var userId = HttpContext.User.UserId();
            var viewModel = await _sectionReviewOrchestrator.GetSectorViewModel(new GetSectorDetailsRequest(applicationId, pageId, userId));
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


        [HttpPost("SectionReview/{applicationId}/Sector/{PageId}")]
        public async Task<IActionResult> ReviewSectorAnswers(Guid applicationId, string pageId, SubmitAssessorPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            Func<Task<SectorViewModel>> viewModelBuilder = () => _sectionReviewOrchestrator.GetSectorViewModel(new GetSectorDetailsRequest(command.ApplicationId, command.PageId, userId));

            return await ValidateAndUpdateSectorPageAnswer(command, viewModelBuilder, $"~/Views/SectionReview/ReviewSectorAnswers.cshtml");
        }
    }
}