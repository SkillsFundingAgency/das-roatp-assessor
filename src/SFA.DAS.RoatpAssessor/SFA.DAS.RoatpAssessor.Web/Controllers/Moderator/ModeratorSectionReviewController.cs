using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.Extensions;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Moderator
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class ModeratorSectionReviewController : ModeratorControllerBase<ModeratorSectionReviewController>
    {
        private readonly IModeratorSectionReviewOrchestrator _sectionReviewOrchestrator;
        public ModeratorSectionReviewController(IRoatpModerationApiClient moderationApiClient,
                                       IModeratorPageValidator moderatorPageValidator,
                                       IModeratorSectionReviewOrchestrator sectionReviewOrchestrator,
                                       ILogger<ModeratorSectionReviewController> logger) : base(moderationApiClient, logger, moderatorPageValidator)
        {
            _sectionReviewOrchestrator = sectionReviewOrchestrator;
        }

        [HttpGet("ModeratorSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}")]
        [HttpGet("ModeratorSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}/Page/{pageId}")]
        public async Task<IActionResult> ReviewPageAnswers(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId)
        {
            var userId = HttpContext.User.UserId();


            if (sequenceNumber == SequenceIds.DeliveringApprenticeshipTraining && sectionNumber == SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees)
            {
                var sectorViewModel = await _sectionReviewOrchestrator.GetSectorsViewModel(new GetSectorsRequest(applicationId, userId));

                if (sectorViewModel is null)
                {
                    return RedirectToAction("ViewApplication", "ModeratorOverview", new { applicationId });
                }

                return View("~/Views/ModeratorSectionReview/ReviewSectors.cshtml", sectorViewModel);
            }

            var viewModel = await _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(applicationId, userId, sequenceNumber, sectionNumber, pageId, null));

            if (viewModel is null)
            {
                return RedirectToAction("ViewApplication", "ModeratorOverview", new { applicationId });
            }

            return View("~/Views/ModeratorSectionReview/ReviewAnswers.cshtml", viewModel);
        }

        [HttpPost("ModeratorSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}")]
        [HttpPost("ModeratorSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}/Page/{pageId}")]
        public async Task<IActionResult> ReviewPageAnswers(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, SubmitModeratorPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();

            Func<Task<ModeratorReviewAnswersViewModel>> viewModelBuilder = () => _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(command.ApplicationId, userId, command.SequenceNumber, command.SectionNumber, command.PageId, command.NextPageId));
            return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"~/Views/ModeratorSectionReview/ReviewAnswers.cshtml");
        }

        [HttpGet("ModeratorSectionReview/{applicationId}/Sector/{PageId}")]
        public async Task<IActionResult> ReviewSectorAnswers(Guid applicationId, string pageId)
        {
            var userId = HttpContext.User.UserId();
            var viewModel = await _sectionReviewOrchestrator.GetSectorDetailsViewModel(new GetSectorDetailsRequest(applicationId, pageId, userId));
            return View("~/Views/ModeratorSectionReview/ReviewSectorAnswers.cshtml", viewModel);
        }

        [HttpPost("ModeratorSectionReview/{applicationId}/Sector/{PageId}")]
        public async Task<IActionResult> ReviewSectorAnswers(Guid applicationId, string pageId, SubmitModeratorPageAnswerCommand command)
        {
            var userId = HttpContext.User.UserId();
            Func<Task<ModeratorSectorDetailsViewModel>> viewModelBuilder = () => _sectionReviewOrchestrator.GetSectorDetailsViewModel(new GetSectorDetailsRequest(command.ApplicationId, command.PageId, userId));

            return await ValidateAndUpdateSectorPageAnswer(command, viewModelBuilder, $"~/Views/ModeratorSectionReview/ReviewSectorAnswers.cshtml");
        }
    }
}