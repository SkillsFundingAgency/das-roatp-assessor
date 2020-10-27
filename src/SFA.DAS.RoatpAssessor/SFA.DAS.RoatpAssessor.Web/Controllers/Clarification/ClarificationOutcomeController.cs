using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Controllers.Moderator;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Clarification
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class ClarificationOutcomeController: Controller
    {
        private readonly IClarificationOutcomeOrchestrator _outcomeOrchestrator;
        protected readonly IClarificationOutcomeValidator _validator;
        protected readonly IRoatpModerationApiClient _moderationApiClient;
        protected readonly ILogger<ClarificationOutcomeController> _logger;

        public ClarificationOutcomeController(IClarificationOutcomeOrchestrator outcomeOrchestrator, IClarificationOutcomeValidator validator, IRoatpModerationApiClient moderationApiClient, ILogger<ClarificationOutcomeController> logger)
        {
            _outcomeOrchestrator = outcomeOrchestrator;
            _validator = validator;
            _moderationApiClient = moderationApiClient;
            _logger = logger;
        }


        [HttpGet("ClarificationOutcome/{applicationId}")]
        public async Task<IActionResult> ViewOutcome(Guid applicationId)
        {
            var userId = HttpContext.User.UserId();

            var viewModel = await _outcomeOrchestrator.GetClarificationOutcomeViewModel(new GetClarificationOutcomeRequest(applicationId, userId));

            if (viewModel is null)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (viewModel.ModerationStatus == ModerationStatus.Pass || viewModel.ModerationStatus == ModerationStatus.Fail)
            {
                // This is in case the user presses the browser back button on AssessmentComplete
                return RedirectToAction("AssessmentComplete", "ClarificationOutcome", new { applicationId });
            }
            else
            {
                return View("~/Views/ClarificationOutcome/Application.cshtml", viewModel);
            }
        }



        [HttpPost("ClarificationOutcome/{applicationId}")]
        public async Task<IActionResult> SubmitClarificationOutcome(Guid applicationId, SubmitClarificationOutcomeCommand command)   //MFCMFC RENAME THIS
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
                var viewModel = await _outcomeOrchestrator.GetClarificationOutcomeViewModel(new GetClarificationOutcomeRequest(applicationId, userId));
                viewModel.OptionPassText = command.OptionPassText;
                viewModel.OptionFailText = command.OptionFailText;
                viewModel.Status = command.Status;

                return View("~/Views/ClarificationOutcome/Application.cshtml", viewModel);
            }

            var viewModelConfirmation = await _outcomeOrchestrator.GetClarificationOutcomeReviewViewModel(
                new ReviewClarificationOutcomeRequest(applicationId, userId, command.Status, command.ReviewComment));

            return View("~/Views/ClarificationOutcome/AreYouSure.cshtml", viewModelConfirmation);

        }


        [HttpPost("ClarificationOutcomeConfirmation/{applicationId}")]
        public async Task<IActionResult> SubmitClarificationOutcomeConfirmation(Guid applicationId, string reviewComment, SubmitClarificationOutcomeConfirmationCommand command)
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
                var viewModel = await _outcomeOrchestrator.GetClarificationOutcomeViewModel(new GetClarificationOutcomeRequest(applicationId, userId));
                viewModel.Status = command.Status;
                switch (command.Status)
                {
                    case ModerationConfirmationStatus.Pass:
                        viewModel.OptionPassText = reviewComment;
                        break;
                    case ModerationConfirmationStatus.Fail:
                        viewModel.OptionFailText = reviewComment;
                        break;
                }

                return View("~/Views/ClarificationOutcome/Application.cshtml", viewModel);
            }

            var userName = HttpContext.User.UserDisplayName();

            var submittedStatus = ModerationStatus.InProgress;

            switch (command.Status)
            {
                case ClarificationConfirmationStatus.Pass:
                    submittedStatus = ModerationStatus.Pass;
                    break;
                case ClarificationConfirmationStatus.Fail:
                    submittedStatus = ModerationStatus.Fail;
                    break;
            }

            var submitSuccessful = await _moderationApiClient.SubmitModerationOutcome(applicationId, userId, userName, submittedStatus, reviewComment);

            if (!submitSuccessful)
            {
                _logger.LogError($"Unable to save moderation outcome for applicationId: [{applicationId}]");
                ModelState.AddModelError(string.Empty, "Unable to save clarification outcome as this time");
                return await GoToErrorView(applicationId, reviewComment, command.Status, userId);
            }

            var viewModelClarificationOutcomeSaved = await _outcomeOrchestrator.GetClarificationOutcomeReviewViewModel(
                new ReviewClarificationOutcomeRequest(applicationId, userId, command.Status, reviewComment));

            var viewModelModerationOutcomeSaved = new ModeratorOutcomeReviewViewModel
            {
                ApplicationId = viewModelClarificationOutcomeSaved.ApplicationId,
                Status = viewModelClarificationOutcomeSaved.Status,
                ApplicantEmailAddress = viewModelClarificationOutcomeSaved.ApplicantEmailAddress,
                ConfirmStatus = viewModelClarificationOutcomeSaved.ConfirmStatus,
                ReviewComment = viewModelClarificationOutcomeSaved.ReviewComment
            };


            return View("~/Views/ModeratorOutcome/ModerationCompleted.cshtml", viewModelModerationOutcomeSaved);

        }

        private async Task<IActionResult> GoToErrorView(Guid applicationId, string reviewComment, string status, string userId)
        {


            if (status == ModerationConfirmationStatus.Pass || status == ModerationConfirmationStatus.Fail || status == ModerationConfirmationStatus.AskForClarification)
            {
                var viewModelPass = await _outcomeOrchestrator.GetClarificationOutcomeReviewViewModel(
                    new ReviewClarificationOutcomeRequest(applicationId, userId, status, reviewComment));
                return View("~/Views/ClarificationOutcome/AreYouSure.cshtml", viewModelPass);
            }

            var viewModel =
                await _outcomeOrchestrator.GetClarificationOutcomeViewModel(
                    new GetClarificationOutcomeRequest(applicationId, userId));

            return View("~/Views/ClarificationOutcome/Application.cshtml", viewModel);
        }
    }
}
