using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAssessorOverviewOrchestrator _overviewOrchestrator;

        public HomeController(IAssessorOverviewOrchestrator overviewOrchestrator)
        {
            _overviewOrchestrator = overviewOrchestrator;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
        }

        public IActionResult Index()
        {
            return RedirectToAction("NewApplications", "Dashboard");
        }

        [HttpGet("Home/{applicationId}")]
        public async Task<IActionResult> ViewApplication(Guid applicationId)
        {
            var username = User.UserDisplayName();

            var viewModel = await _overviewOrchestrator.GetOverviewViewModel(new GetApplicationOverviewRequest(applicationId, username));

            if (viewModel is null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View("~/Views/Home/Application.cshtml", viewModel);

            //TODO: We will need to redirect to read only when approve/declined is implemented
            return View("~/Views/Home/Application_ReadOnly.cshtml", viewModel);
        }
    }
}
