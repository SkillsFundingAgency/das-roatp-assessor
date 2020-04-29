using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAssessorDashboardOrchestrator _orchestrator;
        private readonly IHttpContextAccessor _contextAccessor;

        public HomeController(IAssessorDashboardOrchestrator orchestrator, IHttpContextAccessor contextAccessor)
        {
            _orchestrator = orchestrator;
            _contextAccessor = contextAccessor;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            return RedirectToAction("NewApplications");
        }

        public async Task<IActionResult> NewApplications()
        {
            var userName = _contextAccessor.HttpContext.User.Username();
            userName = "temp"; //Can't access the user until staff idams is enabled
            var vm = await _orchestrator.GetNewApplicationsViewModel(userName);
            return View(vm);
        }
    }
}
