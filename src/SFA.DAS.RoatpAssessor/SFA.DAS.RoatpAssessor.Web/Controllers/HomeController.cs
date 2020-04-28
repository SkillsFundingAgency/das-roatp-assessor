using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAssessorDashboardOrchestrator _orchestrator;

        public HomeController(IAssessorDashboardOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
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
            var vm = await _orchestrator.GetNewApplicationsViewModel("todo");
            return View(vm);
        }
    }
}
