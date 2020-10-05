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

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Moderator
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class ModeratorOutcomeController: Controller
    {
        private readonly IModeratorOutcomeOrchestrator _outcomeOrchestrator;

        public ModeratorOutcomeController(IModeratorOutcomeOrchestrator outcomeOrchestrator)
        {
            _outcomeOrchestrator = outcomeOrchestrator;
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
    }
}
