using System;
using System.Linq;
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

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Moderator
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class ModeratorOutcomeController : Controller
    {
        private readonly IModeratorOutcomeOrchestrator _outcomeOrchestrator;
        protected readonly IModeratorOutcomeValidator _validator;
        protected readonly IRoatpModerationApiClient _moderationApiClient;
        protected readonly ILogger<ModeratorOutcomeController> _logger;
        public ModeratorOutcomeController(IModeratorOutcomeOrchestrator outcomeOrchestrator, IModeratorOutcomeValidator validator, IRoatpModerationApiClient moderationApiClient, ILogger<ModeratorOutcomeController> logger)
        {
            _outcomeOrchestrator = outcomeOrchestrator;
            _validator = validator;
            _moderationApiClient = moderationApiClient;
            _logger = logger;
        }

        [HttpGet("ModeratorOutcome/{applicationId}")]
        public async Task<IActionResult> ViewOutcome(Guid applicationId)
        {
            var userId = HttpContext.User.UserId();

            var viewModel = await _outcomeOrchestrator.GetInModerationOutcomeViewModel(new GetModeratorOutcomeRequest(applicationId, userId));

            if (viewModel is null)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (viewModel.ModerationStatus == ModerationStatus.Pass || viewModel.ModerationStatus == ModerationStatus.Fail)
            {
                // This is in case the user presses the browser back button on AssessmentComplete
                return RedirectToAction("AssessmentComplete", "ModeratorOutcome", new { applicationId });
            }
            else
            {
                return View("~/Views/ModeratorOutcome/Application.cshtml", viewModel);
            }
        }

        [HttpPost("ModeratorOutcome/{applicationId}")]
        public async Task<IActionResult> SubmitModeratorOutcome(Guid applicationId, SubmitModeratorOutcomeCommand command)   //MFCMFC RENAME THIS
        {
            // validate
            var validationResponse = await _validator.Validate(command);
            if (validationResponse.Errors.Any())
            {
                foreach (var error in validationResponse.Errors)
                {
                    ModelState.AddModelError(error.Field, error.ErrorMessage);
                }
            }

            var userId = HttpContext.User.UserId();

            if (!ModelState.IsValid)
            {
                var viewModel = await _outcomeOrchestrator.GetInModerationOutcomeViewModel(new GetModeratorOutcomeRequest(applicationId, userId));
                viewModel.OptionPassText = command.OptionPassText;
                viewModel.OptionFailText = command.OptionFailText;
                viewModel.OptionAskForClarificationText = command.OptionAskForClarificationText;
                viewModel.Status = command.Status;

                return View("~/Views/ModeratorOutcome/Application.cshtml", viewModel);
            }


            var viewModelConfirmation = await _outcomeOrchestrator.GetInModerationOutcomeReviewViewModel(
                new ReviewModeratorOutcomeRequest(applicationId, userId, command.Status, command.ReviewComment));

            return View("~/Views/ModeratorOutcome/AreYouSure.cshtml", viewModelConfirmation);

        }

        [HttpPost("ModeratorOutcomeConfirmation/{applicationId}")]
        public async Task<IActionResult> SubmitModeratorOutcomeConfirmation(Guid applicationId, string reviewComment, SubmitModeratorOutcomeConfirmationCommand command)
        {

            var validationResponse = await _validator.Validate(command);
            if (validationResponse.Errors.Any())
            {
                foreach (var error in validationResponse.Errors)
                {
                    ModelState.AddModelError(error.Field, error.ErrorMessage);
                }
            }

            var userId = HttpContext.User.UserId();

            if (!ModelState.IsValid)
            {
                return await GoToErrorView(applicationId, reviewComment, command.Status, userId);
            }

            if (command.ConfirmStatus == "No")
            {
                var viewModel = await _outcomeOrchestrator.GetInModerationOutcomeViewModel(new GetModeratorOutcomeRequest(applicationId, userId));
                viewModel.Status = command.Status;
                switch (command.Status)
                {
                    case ModerationConfirmationStatus.Pass:
                        viewModel.OptionPassText = reviewComment;
                        break;
                    case ModerationConfirmationStatus.Fail:
                        viewModel.OptionFailText = reviewComment;
                        break;
                    case ModerationConfirmationStatus.AskForClarification:
                        viewModel.OptionAskForClarificationText = reviewComment;
                        break;
                }

                return View("~/Views/ModeratorOutcome/Application.cshtml", viewModel);
            }

            var userName = HttpContext.User.UserDisplayName();

            var submittedStatus = ModerationStatus.InProgress;

            switch (command.Status)
            {
                case ModerationConfirmationStatus.Pass:
                    submittedStatus = ModerationStatus.Pass;
                    break;
                case ModerationConfirmationStatus.Fail:
                    submittedStatus = ModerationStatus.Fail;
                    break;
                case ModerationConfirmationStatus.AskForClarification:
                    submittedStatus = ModerationStatus.ClarificationSent;
                    break;
            }

            var submitSuccessful = await _moderationApiClient.SubmitModerationOutcome(applicationId, userId, userName, submittedStatus, reviewComment);

            if (!submitSuccessful)
            {
                _logger.LogError($"Unable to save moderation outcome for applicationId: [{applicationId}]");
                ModelState.AddModelError(string.Empty, "Unable to save moderation outcome as this time");
                return await GoToErrorView(applicationId, reviewComment, command.Status, userId);
            }

            var viewModelModerationOutcomeSaved = await _outcomeOrchestrator.GetInModerationOutcomeReviewViewModel(
                new ReviewModeratorOutcomeRequest(applicationId, userId, command.Status, reviewComment));
            return View("~/Views/ModeratorOutcome/ModerationCompleted.cshtml", viewModelModerationOutcomeSaved);

        }

        private async Task<IActionResult> GoToErrorView(Guid applicationId, string reviewComment, string status, string userId)
        {


            if (status == ModerationConfirmationStatus.Pass || status == ModerationConfirmationStatus.Fail || status == ModerationConfirmationStatus.AskForClarification)
            {
                var viewModelPass = await _outcomeOrchestrator.GetInModerationOutcomeReviewViewModel(
                    new ReviewModeratorOutcomeRequest(applicationId, userId, status, reviewComment));
                return View("~/Views/ModeratorOutcome/AreYouSure.cshtml", viewModelPass);
            }

            var viewModel =
                await _outcomeOrchestrator.GetInModerationOutcomeViewModel(
                    new GetModeratorOutcomeRequest(applicationId, userId));

            return View("~/Views/ModeratorOutcome/Application.cshtml", viewModel);
        }
    }
}
