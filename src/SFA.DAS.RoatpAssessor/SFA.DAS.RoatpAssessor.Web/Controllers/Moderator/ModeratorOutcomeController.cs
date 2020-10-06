using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Moderator
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class ModeratorOutcomeController: Controller
    {
        private readonly IModeratorOutcomeOrchestrator _outcomeOrchestrator;
        protected readonly IModeratorOutcomeValidator _validator;
        public ModeratorOutcomeController(IModeratorOutcomeOrchestrator outcomeOrchestrator, IModeratorOutcomeValidator validator)
        {
            _outcomeOrchestrator = outcomeOrchestrator;
            _validator = validator;
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
            else if (viewModel.ModerationStatus == ModerationStatus.Complete)
            {
                return RedirectToAction("AssessmentComplete", "ModeratorOutcome", new { applicationId });
            }

            return View("~/Views/ModeratorOutcome/Application.cshtml", viewModel);
        }



        [HttpPost("ModeratorOutcome/{applicationId}")]
        public async Task<IActionResult> SubmitOutcome(Guid applicationId, SubmitModeratorOutcomeCommand command)
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
             
                var viewModelOutcome = await _outcomeOrchestrator.GetInModerationOutcomeViewModel(new GetModeratorOutcomeRequest(applicationId, userId));
                viewModelOutcome.OptionPassText = command.OptionPassText;
                viewModelOutcome.OptionFailText = command.OptionFailText;
                viewModelOutcome.OptionAskForClarificationText = command.OptionAskForClarificationText;
                viewModelOutcome.Status = command.Status;
                return View("~/Views/ModeratorOutcome/Application.cshtml", viewModelOutcome);
            }

            var request =
                new ReviewModeratorOutcomeRequest(applicationId, userId, command.Status, command.ReviewComment);
            var viewModel = await _outcomeOrchestrator.GetInModerationOutcomeReviewViewModel(request);

            return View("~/Views/ModeratorOutcome/AreYouSure.cshtml", viewModel);

        }
    }
}
