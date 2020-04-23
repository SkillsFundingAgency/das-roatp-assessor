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
        private readonly IAssessorOverviewOrchestrator _overviewOrchestrator;

        public HomeController(IAssessorOverviewOrchestrator overviewOrchestrator)
        {
            _overviewOrchestrator = overviewOrchestrator;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
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
