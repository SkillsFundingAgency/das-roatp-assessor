using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Services;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IAssessorDashboardOrchestrator _orchestrator;
        private readonly IHttpContextAccessor _contextAccessor;
        
        public DashboardController(IAssessorDashboardOrchestrator orchestrator, IHttpContextAccessor contextAccessor)
        {
            _orchestrator = orchestrator;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            return RedirectToAction("NewApplications");
        }

        public async Task<IActionResult> NewApplications()
        {
            var userId = _contextAccessor.HttpContext.User.UserId();
            userId = "temp"; //TODO: Can't access the user until staff idams is enabled
            var vm = await _orchestrator.GetNewApplicationsViewModel(userId);
            return View(vm);
        }

        public async Task<IActionResult> AssignToAssessor(Guid applicationId, int assessorNumber)
        {
            var userId = _contextAccessor.HttpContext.User.UserId();
            var userName = _contextAccessor.HttpContext.User.UserDisplayName();

            userId = "temp"; //TODO: Can't access the user until staff idams is enabled
            userName = "Joe Bloggs"; //TODO: Can't access the user until staff idams is enabled

            await _orchestrator.AssignApplicationToAssessor(applicationId, assessorNumber, userId, userName);

            return RedirectToAction("ViewApplication", "Home", new { applicationId });
        }

        public async Task<IActionResult> InProgressApplications()
        {
            var userId = _contextAccessor.HttpContext.User.UserId();
            userId = "temp"; //TODO: Can't access the user until staff idams is enabled
            var vm = await _orchestrator.GetInProgressApplicationsViewModel(userId);
            return View(vm);
        }
    }
}