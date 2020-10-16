using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.Domain;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Clarification
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class ClarificationOverviewController : Controller
    {

        public ClarificationOverviewController()
        {
        }

        [HttpGet("ClarificationOverview/{applicationId}")]
        public async Task<IActionResult> ViewApplication(Guid applicationId)
        {
            // Placeholder for now. 
            // TODO: Add orchestrator and return appropriate the view or redirect action

            var userId = HttpContext.User.UserId();

            await Task.Delay(250); // Pretend to load something

            return View("~/Views/ClarificationOverview/Application.cshtml", new { });
        }
    }
}
