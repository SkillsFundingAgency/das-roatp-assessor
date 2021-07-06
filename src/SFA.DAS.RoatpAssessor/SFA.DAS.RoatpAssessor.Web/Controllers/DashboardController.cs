﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.ModelBinders;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class DashboardController : Controller
    {
        private readonly ISearchTermValidator _searchTermValidator;
        private readonly IAssessorDashboardOrchestrator _assessorOrchestrator;
        private readonly IModeratorDashboardOrchestrator _moderatorOrchestrator;
        private readonly IClarificationDashboardOrchestrator _clarificationOrchestrator;
        private readonly IOutcomeDashboardOrchestrator _outcomeOrchestrator;

        public DashboardController(ISearchTermValidator searchTermValidator, IAssessorDashboardOrchestrator orchestrator, IModeratorDashboardOrchestrator moderatorOrchestrator, IClarificationDashboardOrchestrator clarificationOrchestrator, IOutcomeDashboardOrchestrator outcomeOrchestrator)
        {
            _searchTermValidator = searchTermValidator;
            _assessorOrchestrator = orchestrator;
            _moderatorOrchestrator = moderatorOrchestrator;
            _clarificationOrchestrator = clarificationOrchestrator;
            _outcomeOrchestrator = outcomeOrchestrator;
        }

        [HttpGet("/Dashboard/New")]
        public async Task<ViewResult> NewApplications([StringTrim] string searchTerm, string sortColumn, string sortOrder)
        {
            ValidateSearchTerm(searchTerm);
            ViewModels.NewApplicationsViewModel vm;

            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.UserId();
                vm = await _assessorOrchestrator.GetNewApplicationsViewModel(userId, searchTerm, sortColumn, sortOrder);
            }
            else
            {
                vm = new ViewModels.NewApplicationsViewModel(0, 0, 0, 0, 0)
                {
                    SearchTerm = searchTerm,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder
                };
            }

            return View(vm);
        }

        [Route("/Dashboard/AssignToAssessor")]
        public async Task<IActionResult> AssignToAssessor(Guid applicationId, int assessorNumber)
        {
            var userId = HttpContext.User.UserId();
            var userName = HttpContext.User.UserDisplayName();

            await _assessorOrchestrator.AssignApplicationToAssessor(applicationId, assessorNumber, userId, userName);

            return RedirectToAction("ViewApplication", "AssessorOverview", new { applicationId });
        }

        [HttpGet("/Dashboard/InProgress")]
        public async Task<ViewResult> InProgressApplications([StringTrim] string searchTerm, string sortColumn, string sortOrder)
        {
            ValidateSearchTerm(searchTerm);
            ViewModels.InProgressApplicationsViewModel vm;

            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.UserId();
                vm = await _assessorOrchestrator.GetInProgressApplicationsViewModel(userId, searchTerm, sortColumn, sortOrder);
            }
            else
            {
                vm = new ViewModels.InProgressApplicationsViewModel(HttpContext.User.UserId(), 0, 0, 0, 0, 0)
                {
                    SearchTerm = searchTerm,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder
                };
            }

            return View(vm);
        }

        [HttpGet("/Dashboard/InModeration")]
        public async Task<ViewResult> InModerationApplications([StringTrim] string searchTerm, string sortColumn, string sortOrder)
        {
            ValidateSearchTerm(searchTerm);
            ViewModels.InModerationApplicationsViewModel vm;

            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.UserId();
                vm = await _moderatorOrchestrator.GetInModerationApplicationsViewModel(userId, searchTerm, sortColumn, sortOrder);
            }
            else
            {
                vm = new ViewModels.InModerationApplicationsViewModel(HttpContext.User.UserId(), 0, 0, 0, 0, 0)
                {
                    SearchTerm = searchTerm,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder
                };
            }

            return View(vm);
        }

        [HttpGet("/Dashboard/InClarification")]
        public async Task<ViewResult> InClarificationApplications([StringTrim] string searchTerm, string sortColumn, string sortOrder)
        {
            ValidateSearchTerm(searchTerm);
            ViewModels.InClarificationApplicationsViewModel vm;

            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.UserId();
                vm = await _clarificationOrchestrator.GetInClarificationApplicationsViewModel(userId, searchTerm, sortColumn, sortOrder);
            }
            else
            {
                vm = new ViewModels.InClarificationApplicationsViewModel(HttpContext.User.UserId(), 0, 0, 0, 0, 0)
                {
                    SearchTerm = searchTerm,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder
                };
            }

            return View(vm);
        }

        [HttpGet("/Dashboard/Outcome")]
        public async Task<ViewResult> ClosedApplications([StringTrim] string searchTerm, string sortColumn, string sortOrder)
        {
            ValidateSearchTerm(searchTerm);
            ViewModels.ClosedApplicationsViewModel vm;

            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.UserId();
                vm = await _outcomeOrchestrator.GetClosedApplicationsViewModel(userId, searchTerm, sortColumn, sortOrder);
            }
            else
            {
                vm = new ViewModels.ClosedApplicationsViewModel(HttpContext.User.UserId(), 0, 0, 0, 0, 0)
                {
                    SearchTerm = searchTerm,
                    SortColumn = sortColumn,
                    SortOrder = sortOrder
                };
            }

            return View(vm);
        }

        private void ValidateSearchTerm(string searchTerm)
        {
            if (searchTerm != null)
            {
                var validationResponse = _searchTermValidator.Validate(searchTerm);

                foreach (var error in validationResponse.Errors)
                {
                    ModelState.AddModelError(error.Field, error.ErrorMessage);
                }
            }
        }
    }
}