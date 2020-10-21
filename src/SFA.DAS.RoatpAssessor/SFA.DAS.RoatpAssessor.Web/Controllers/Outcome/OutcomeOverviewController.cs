using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Outcome
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class OutcomeOverviewController: Controller
    {
        private readonly IOutcomeOverviewOrchestrator _outcomeOrchestrator;
        protected readonly IRoatpModerationApiClient _moderationApiClient;
        protected readonly ILogger<OutcomeOverviewController> _logger;

        public OutcomeOverviewController(IOutcomeOverviewOrchestrator outcomeOrchestrator, IRoatpModerationApiClient moderationApiClient, ILogger<OutcomeOverviewController> logger)
        {
            _outcomeOrchestrator = outcomeOrchestrator;
            _moderationApiClient = moderationApiClient;
            _logger = logger;
        }



        [HttpGet("OutcomeOverviewController/{applicationId}")]
        public async Task<IActionResult> ViewOutcome(Guid applicationId)
        {
            var userId = HttpContext.User.UserId();

            var viewModel = await _outcomeOrchestrator.GetOverviewViewModel(new GetOutcomeOverviewRequest(applicationId, userId));

            if (viewModel is null)
            {
                return RedirectToAction("Index", "Home");
            }

                return View("~/Views/OutcomeOverview/Application.cshtml", viewModel);
            
        }
    }
}
