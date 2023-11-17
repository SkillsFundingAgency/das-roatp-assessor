using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.Extensions;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Clarification
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class ClarificationOverviewController : Controller
    {
        private readonly IClarificationOverviewOrchestrator _overviewOrchestrator;

        public ClarificationOverviewController(IClarificationOverviewOrchestrator overviewOrchestrator)
        {
            _overviewOrchestrator = overviewOrchestrator;
        }

        [HttpGet("ClarificationOverview/{applicationId}")]
        public async Task<IActionResult> ViewApplication(Guid applicationId)
        {
            var userId = HttpContext.User.UserId();

            var viewModel = await _overviewOrchestrator.GetOverviewViewModel(new GetClarificationOverviewRequest(applicationId, userId));

            if (viewModel is null)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (viewModel.ModerationStatus == ModerationStatus.Pass || viewModel.ModerationStatus == ModerationStatus.Fail)
            {
                return RedirectToAction("AssessmentComplete", "ClarificationOutcome", new { applicationId });
            }

            return View("~/Views/ClarificationOverview/Application.cshtml", viewModel);
        }
    }
}
