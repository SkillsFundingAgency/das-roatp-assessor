using System;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly AssessorDashboardOrchestrator _orchestrator;

        public HomeController(AssessorDashboardOrchestrator orchestrator)
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

        public IActionResult NewApplications()
        {
            var vm = _orchestrator.GetNewApplicationsViewModel("todo");
            return View(vm);
        }
    }
}
