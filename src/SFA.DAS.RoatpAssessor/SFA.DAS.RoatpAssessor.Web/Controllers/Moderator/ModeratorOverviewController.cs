using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Extensions;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Moderator
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class ModeratorOverviewController : Controller
    {
        private readonly IModeratorOverviewOrchestrator _overviewOrchestrator;
        private readonly ILogger<ModeratorOverviewController> _logger;

        public ModeratorOverviewController(IModeratorOverviewOrchestrator overviewOrchestrator, ILogger<ModeratorOverviewController> logger)
        {
            _overviewOrchestrator = overviewOrchestrator;
            _logger = logger;
        }

        [HttpGet("ModeratorOverview/{applicationId}")]
        public async Task<IActionResult> ViewApplication(Guid applicationId)
        {
            _logger.LogInformation("Provider Moderation TEST TEST TEST");
            var userId = HttpContext.User.UserId();

            var viewModel = await _overviewOrchestrator.GetOverviewViewModel(new GetModeratorOverviewRequest(applicationId, userId));

            if (viewModel is null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (viewModel.ModerationStatus == ModerationStatus.Pass || viewModel.ModerationStatus == ModerationStatus.Fail)
            {
                return RedirectToAction("AssessmentComplete", "ModeratorOutcome", new { applicationId });
            }

            return View("~/Views/ModeratorOverview/Application.cshtml", viewModel);
        }
    }
}
