using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Services;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class DashboardController : Controller
    {
        private readonly IAssessorDashboardOrchestrator _assessorOrchestrator;
        private readonly IModeratorDashboardOrchestrator _moderatorOrchestrator;

        public DashboardController(IAssessorDashboardOrchestrator orchestrator, IModeratorDashboardOrchestrator moderatorOrchestrator)
        {
            _assessorOrchestrator = orchestrator;
            _moderatorOrchestrator = moderatorOrchestrator;
        }

        public IActionResult Index()
        {
            return RedirectToAction("NewApplications");
        }

        public async Task<ViewResult> NewApplications()
        {
            var userId = HttpContext.User.UserId();
            var vm = await _assessorOrchestrator.GetNewApplicationsViewModel(userId);
            return View(vm);
        }

        public async Task<IActionResult> AssignToAssessor(Guid applicationId, int assessorNumber)
        {
            var userId = HttpContext.User.UserId();
            var userName = HttpContext.User.UserDisplayName();

            await _assessorOrchestrator.AssignApplicationToAssessor(applicationId, assessorNumber, userId, userName);

            return RedirectToAction("ViewApplication", "Overview", new { applicationId });
        }

        public async Task<ViewResult> InProgressApplications()
        {
            var userId = HttpContext.User.UserId();
            var vm = await _assessorOrchestrator.GetInProgressApplicationsViewModel(userId);
            return View(vm);
        }

        public async Task<ViewResult> InModerationApplications()
        {
            var userId = HttpContext.User.UserId();
            var vm = await _moderatorOrchestrator.GetInModerationApplicationsViewModel(userId);
            return View(vm);
        }
    }
}