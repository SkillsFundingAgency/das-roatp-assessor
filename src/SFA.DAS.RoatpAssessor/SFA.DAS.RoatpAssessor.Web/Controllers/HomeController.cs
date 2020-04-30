using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAssessorDashboardOrchestrator _orchestrator;
        private readonly IHttpContextAccessor _contextAccessor;
		private readonly IAssessorOverviewOrchestrator _overviewOrchestrator;

        public HomeController(IAssessorOverviewOrchestrator overviewOrchestrator, IAssessorDashboardOrchestrator orchestrator, IHttpContextAccessor contextAccessor)
        {
            _overviewOrchestrator = overviewOrchestrator;
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

            return RedirectToAction("NewApplications");
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

            switch (viewModel.AssessorReviewStatus)
            {
                case AssessorReviewStatus.New:
                case AssessorReviewStatus.InProgress:
                    return View("~/Views/Home/Application.cshtml", viewModel);
                case AssessorReviewStatus.Approved:
                case AssessorReviewStatus.Declined:
                    return View("~/Views/Home/Application_ReadOnly.cshtml", viewModel);
                default:
                    return RedirectToAction(nameof(Index));
            }
        }
    }
}
