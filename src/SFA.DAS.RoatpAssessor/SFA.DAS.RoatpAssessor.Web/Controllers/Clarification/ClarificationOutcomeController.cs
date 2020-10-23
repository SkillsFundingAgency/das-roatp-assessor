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
    }
}
