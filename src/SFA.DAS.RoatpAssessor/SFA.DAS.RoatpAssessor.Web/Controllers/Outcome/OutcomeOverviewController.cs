using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Outcome
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class OutcomeOverviewController: Controller
    {
        private readonly IOutcomeOverviewOrchestrator _overviewOrchestrator;

        public OutcomeOverviewController(IOutcomeOverviewOrchestrator overviewOrchestrator)
        {
            _overviewOrchestrator = overviewOrchestrator;
        }

        [HttpGet("OutcomeOverview/{applicationId}")]
        public async Task<IActionResult> ViewApplication(Guid applicationId)
        {
            var userId = HttpContext.User.UserId();
            var viewModel = await _overviewOrchestrator.GetOverviewViewModel(new GetOutcomeOverviewRequest(applicationId, userId));

            if (viewModel is null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("~/Views/OutcomeOverview/Application.cshtml", viewModel);
        }
    }
}
