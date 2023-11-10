using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Extensions;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Assessor
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class AssessorSectionReviewController : AssessorControllerBase<AssessorSectionReviewController>
    {
        private readonly IAssessorSectionReviewOrchestrator _sectionReviewOrchestrator;
        public AssessorSectionReviewController(IRoatpAssessorApiClient assessorApiClient,
                                       IAssessorPageValidator assessorPageValidator,
                                       IAssessorSectionReviewOrchestrator sectionReviewOrchestrator,
                                       ILogger<AssessorSectionReviewController> logger) : base(assessorApiClient, logger, assessorPageValidator)
        {
            _sectionReviewOrchestrator = sectionReviewOrchestrator;
        }

        [HttpGet("AssessorSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}")]
        [HttpGet("AssessorSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}/Page/{pageId}")]
        public async Task<IActionResult> ReviewPageAnswers(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId)
        {
            var userId = HttpContext.User.UserId();


            if (sequenceNumber == SequenceIds.DeliveringApprenticeshipTraining && sectionNumber == SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees)
            {
                var sectorViewModel = await _sectionReviewOrchestrator.GetSectorsViewModel(new GetSectorsRequest(applicationId, userId));

                if (sectorViewModel is null)
                {
                    return RedirectToAction("ViewApplication", "AssessorOverview", new { applicationId });
                }

                return View("~/Views/AssessorSectionReview/ReviewSectors.cshtml", sectorViewModel);
            }

            var viewModel = await _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(applicationId, userId, sequenceNumber, sectionNumber, pageId, null));

            if (viewModel is null)
            {
                return RedirectToAction("ViewApplication", "AssessorOverview", new { applicationId });
            }

            return View("~/Views/AssessorSectionReview/ReviewAnswers.cshtml", viewModel);
        }

        [HttpPost("AssessorSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}")]
        [HttpPost("AssessorSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}/Page/{pageId}")]
        public async Task<IActionResult> ReviewPageAnswers(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, SubmitAssessorPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();

            Func<Task<AssessorReviewAnswersViewModel>> viewModelBuilder = () => _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(command.ApplicationId, userId, command.SequenceNumber, command.SectionNumber, command.PageId, command.NextPageId));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"~/Views/AssessorSectionReview/ReviewAnswers.cshtml");
        }

        [HttpGet("AssessorSectionReview/{applicationId}/Sector/{PageId}")]
        public async Task<IActionResult> ReviewSectorAnswers(Guid applicationId, string pageId)
        {
            var userId = HttpContext.User.UserId();
            var viewModel = await _sectionReviewOrchestrator.GetSectorDetailsViewModel(new GetSectorDetailsRequest(applicationId, pageId, userId));
            return View("~/Views/AssessorSectionReview/ReviewSectorAnswers.cshtml", viewModel);
        }

        [HttpPost("AssessorSectionReview/{applicationId}/Sector/{PageId}")]
        public async Task<IActionResult> ReviewSectorAnswers(Guid applicationId, string pageId, SubmitAssessorPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            Func<Task<AssessorSectorDetailsViewModel>> viewModelBuilder = () => _sectionReviewOrchestrator.GetSectorDetailsViewModel(new GetSectorDetailsRequest(command.ApplicationId, command.PageId, userId));

            return await ValidateAndUpdateSectorPageAnswer(command, viewModelBuilder, $"~/Views/AssessorSectionReview/ReviewSectorAnswers.cshtml");
        }
    }
}