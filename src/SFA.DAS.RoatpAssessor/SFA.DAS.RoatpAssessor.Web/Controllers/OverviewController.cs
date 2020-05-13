using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Services;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
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
            var username = User.UserDisplayName();

            var viewModel = await _overviewOrchestrator.GetOverviewViewModel(new GetApplicationOverviewRequest(applicationId, username));

            if (viewModel is null)
            {
                return RedirectToAction("NewApplications", "Dashboard");
            }

            return View("~/Views/Overview/Application.cshtml", viewModel);

            //TODO: We will need to redirect to read only when approve/declined is implemented
            return View("~/Views/Overview/Application_ReadOnly.cshtml", viewModel);
        }
    }
}
