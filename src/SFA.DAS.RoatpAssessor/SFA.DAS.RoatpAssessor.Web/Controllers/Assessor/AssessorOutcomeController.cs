﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Extensions;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Assessor
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class AssessorOutcomeController : Controller
    {
        private readonly IRoatpAssessorApiClient _assessorApiClient;
        private readonly IAssessorOverviewOrchestrator _overviewOrchestrator;
        private readonly IAssessorOutcomeValidator _assessorOutcomeValidator;

        public AssessorOutcomeController(IRoatpAssessorApiClient assessorApiClient, IAssessorOverviewOrchestrator overviewOrchestrator, IAssessorOutcomeValidator assessorOutcomeValidator)
        {
            _assessorApiClient = assessorApiClient;
            _overviewOrchestrator = overviewOrchestrator;
            _assessorOutcomeValidator = assessorOutcomeValidator;
        }

        [HttpGet("AssessorOutcome/{applicationId}")]
        public async Task<IActionResult> AssessorOutcome(Guid applicationId)
        {
            var userId = HttpContext.User.UserId();

            var viewModel = await _overviewOrchestrator.GetOverviewViewModel(new GetAssessorOverviewRequest(applicationId, userId));

            if (viewModel is null || viewModel.IsAssessorApproved)
            {
                // This is in case the user presses the browser back button on AssessmentComplete
                return RedirectToAction("Index", "Home");
            }
            else if (!viewModel.IsReadyForModeration)
            {
                return RedirectToAction("ViewApplication", "AssessorOverview", new { applicationId });
            }
            else
            {
                return View("~/Views/AssessorOutcome/AssessorOutcome.cshtml", viewModel);
            }
        }

        [HttpPost("AssessorOutcome/{applicationId}")]
        public async Task<IActionResult> AssessorOutcome(Guid applicationId, SubmitAssessorOutcomeCommand command)
        {
            // validate
            var validationResponse = await _assessorOutcomeValidator.Validate(command);

            if (validationResponse.Errors.Any())
            {
                foreach (var error in validationResponse.Errors)
                {
                    ModelState.AddModelError(error.Field, error.ErrorMessage);
                }
            }

            var userId = HttpContext.User.UserId();
            var userName = HttpContext.User.UserDisplayName();
            var submitForModeration = "YES".Equals(command.MoveToModeration, StringComparison.OrdinalIgnoreCase);

            // submit if validation passed and user specified to do so
            if (ModelState.IsValid && submitForModeration)
            {
                var submittedSuccessfully = await _assessorApiClient.UpdateAssessorReviewStatus(command.ApplicationId, userId, userName, AssessorReviewStatus.Approved);

                if (!submittedSuccessfully)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save at this time");
                }
            }

            // redirect
            if (!ModelState.IsValid)
            {
                var viewModel = await _overviewOrchestrator.GetOverviewViewModel(new GetAssessorOverviewRequest(applicationId, userId));
                return View("~/Views/AssessorOutcome/AssessorOutcome.cshtml", viewModel);
            }
            else if (!submitForModeration)
            {
                return RedirectToAction("ViewApplication", "AssessorOverview", new { applicationId });
            }
            else
            {
                return RedirectToAction("AssessmentComplete", "AssessorOutcome", new { applicationId = command.ApplicationId });
            }
        }

        [HttpGet("AssessorOutcome/{applicationId}/AssessmentComplete")]
        public async Task<IActionResult> AssessmentComplete(Guid applicationId)
        {
            var userId = HttpContext.User.UserId();

            var viewModel = await _overviewOrchestrator.GetOverviewViewModel(new GetAssessorOverviewRequest(applicationId, userId));

            if (viewModel is null || !viewModel.IsAssessorApproved)
            {
                return RedirectToAction("ViewApplication", "AssessorOverview", new { applicationId });
            }

            return View("~/Views/AssessorOutcome/AssessmentComplete.cshtml", viewModel);
        }
    }
}