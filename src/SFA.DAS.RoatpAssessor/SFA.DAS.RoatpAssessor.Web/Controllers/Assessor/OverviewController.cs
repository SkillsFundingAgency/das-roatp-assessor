using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Assessor
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class OverviewController : Controller
    {
        private readonly IAssessorOverviewOrchestrator _overviewOrchestrator;

        public OverviewController(IAssessorOverviewOrchestrator overviewOrchestrator)
        {
            _overviewOrchestrator = overviewOrchestrator;
        }

        [HttpGet("Overview/{applicationId}")]
        public async Task<IActionResult> ViewApplication(Guid applicationId)
        {
            var userId = HttpContext.User.UserId();

            var viewModel = await _overviewOrchestrator.GetOverviewViewModel(new GetAssessorOverviewRequest(applicationId, userId));

            if (viewModel is null)
            {
                return RedirectToAction("Index", "Home");
            }
            else if (viewModel.IsAssessorApproved)
            {
                return RedirectToAction("AssessmentComplete", "AssessorOutcome", new { applicationId });
            }

            return View("~/Views/Overview/Application.cshtml", viewModel);
        }
    }
}
