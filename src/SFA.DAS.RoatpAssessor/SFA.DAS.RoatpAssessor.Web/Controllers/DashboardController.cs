using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Settings;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IAssessorDashboardOrchestrator _orchestrator;
        private readonly IWebConfiguration _configuration;
        public DashboardController(IAssessorDashboardOrchestrator orchestrator, IWebConfiguration configuration)
        {
            _orchestrator = orchestrator;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return RedirectToAction("NewApplications");
        }

        public async Task<ViewResult> NewApplications()
        {
            var userId = HttpContext.User.UserId();
            var vm = await _orchestrator.GetNewApplicationsViewModel(userId);
            return View(vm);
        }

        public async Task<IActionResult> AssignToAssessor(Guid applicationId, int assessorNumber)
        {
            var userId = HttpContext.User.UserId();
            var userName = HttpContext.User.UserDisplayName();

            await _orchestrator.AssignApplicationToAssessor(applicationId, assessorNumber, userId, userName);

            return RedirectToAction("ViewApplication", "Overview", new { applicationId });
        }

        public async Task<ViewResult> InProgressApplications()
        {
            var userId = HttpContext.User.UserId();
            var vm = await _orchestrator.GetInProgressApplicationsViewModel(userId);
            return View(vm);
        }


        [Route("/Dashboard")]
        public IActionResult Dashboard()
        {
            return Redirect(_configuration.EsfaAdminServicesBaseUrl + "/Dashboard");
        }
    }
}