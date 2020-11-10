using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Outcome
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class OutcomeSectionReviewController : Controller
    {
        private readonly IOutcomeSectionReviewOrchestrator _sectionReviewOrchestrator;

        public OutcomeSectionReviewController(IOutcomeSectionReviewOrchestrator sectionReviewOrchestrator)
        {
            _sectionReviewOrchestrator = sectionReviewOrchestrator;
        }

        [HttpGet("OutcomeSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}")]
        [HttpGet("OutcomeSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}/Page/{pageId}")]
        public async Task<IActionResult> ReviewPageAnswers(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId)
        {
            var userId = HttpContext.User.UserId();

            if (sequenceNumber == SequenceIds.DeliveringApprenticeshipTraining && sectionNumber == SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees)
            {
                var sectorViewModel = await _sectionReviewOrchestrator.GetSectorsViewModel(new GetSectorsRequest(applicationId, userId));

                if (sectorViewModel is null)
                {
                    return RedirectToAction("ViewApplication", "OutcomeOverview", new { applicationId });
                }

                return View("~/Views/OutcomeSectionReview/ReviewSectors.cshtml", sectorViewModel);
            }

            var viewModel = await _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(applicationId, userId, sequenceNumber, sectionNumber, pageId, null));

            if (viewModel is null)
            {
                return RedirectToAction("ViewApplication", "OutcomeOverview", new { applicationId });
            }

            return View("~/Views/OutcomeSectionReview/ReviewAnswers.cshtml", viewModel);
        }

        [HttpPost("OutcomeSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}")]
        [HttpPost("OutcomeSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}/Page/{pageId}")]
        public IActionResult ReviewPageAnswers(SubmitOutcomePageAnswerCommand command)
        {
            if(string.IsNullOrEmpty(command.NextPageId))
            {
                return RedirectToAction("ViewApplication", "OutcomeOverview", new { applicationId = command.ApplicationId }, $"sequence-{command.SequenceNumber}");
            }
            else
            {
                return RedirectToAction("ReviewPageAnswers", "OutcomeSectionReview", new { applicationId = command.ApplicationId, sequenceNumber = command.SequenceNumber, sectionNumber = command.SectionNumber, pageId = command.NextPageId });
            }
        }

        [HttpGet("OutcomeSectionReview/{applicationId}/Sector/{pageId}")]
        public async Task<IActionResult> ReviewSectorAnswers(Guid applicationId, string pageId)
        {
            var userId = HttpContext.User.UserId();
            var viewModel = await _sectionReviewOrchestrator.GetSectorDetailsViewModel(new GetSectorDetailsRequest(applicationId, pageId, userId));
            return View("~/Views/OutcomeSectionReview/ReviewSectorAnswers.cshtml", viewModel);
        }

        [HttpPost("OutcomeSectionReview/{applicationId}/Sector/{pageId}")]
        public IActionResult ReviewSectorAnswers(SubmitOutcomePageAnswerCommand command)
        {
            return RedirectToAction("ReviewPageAnswers", "OutcomeSectionReview", new
                {
                    applicationId = command.ApplicationId,
                    sequenceNumber = SequenceIds.DeliveringApprenticeshipTraining,
                    sectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees
                });
        }
    }
}