using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Clarification
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class ClarificationControllerBase<T> : Controller where T : class
    {
        protected readonly IRoatpClarificationApiClient _clarificationApiClient;
        protected readonly ILogger<T> _logger;
        protected readonly IClarificationPageValidator _clarificationPageValidator;

        public ClarificationControllerBase(IRoatpClarificationApiClient clarificationApiClient, ILogger<T> logger, IClarificationPageValidator clarificationPageValidator)
        {
            _clarificationApiClient = clarificationApiClient;
            _logger = logger;
            _clarificationPageValidator = clarificationPageValidator;
        }

        protected async Task<IActionResult> ValidateAndUpdatePageAnswer<RAVM>(SubmitClarificationPageAnswerCommand command,
                                                          Func<Task<RAVM>> viewModelBuilder,
                                                          string errorView) where RAVM : ClarifierReviewAnswersViewModel
        {
            var validationResponse = await _clarificationPageValidator.Validate(command);

            if (validationResponse.Errors.Any())
            {
                foreach (var error in validationResponse.Errors)
                {
                    ModelState.AddModelError(error.Field, error.ErrorMessage);
                }
            }

            var submittedPageOutcomeSuccessfully = false;

            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.UserId();
                var userName = HttpContext.User.UserDisplayName();

                submittedPageOutcomeSuccessfully = await _clarificationApiClient.SubmitClarificationPageReviewOutcome(command.ApplicationId,
                                    command.SequenceNumber,
                                    command.SectionNumber,
                                    command.PageId,
                                    userId,
                                    userName,
                                    command.ClarificationResponse,
                                    command.Status,
                                    command.ReviewComment,
                                    command.FilesToUpload);

                if (!submittedPageOutcomeSuccessfully)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save outcome as this time");
                }
            }

            if (!submittedPageOutcomeSuccessfully)
            {
                var viewModel = await viewModelBuilder.Invoke();
                viewModel.Status = command.Status;
                viewModel.OptionFailText = command.OptionFailText;
                viewModel.OptionInProgressText = command.OptionInProgressText;
                viewModel.OptionPassText = command.OptionPassText;

                viewModel.ClarificationResponse = command.ClarificationResponse;

                return View(errorView, viewModel);
            }
            else if (string.IsNullOrEmpty(command.NextPageId))
            {
                return RedirectToAction("ViewApplication", "ClarificationOverview", new { applicationId = command.ApplicationId }, $"sequence-{command.SequenceNumber}");
            }
            else
            {
                return RedirectToAction("ReviewPageAnswers", new { applicationId = command.ApplicationId, sequenceNumber = command.SequenceNumber, sectionNumber = command.SectionNumber, pageId = command.NextPageId });
            }
        }

        protected async Task<IActionResult> ValidateAndUpdateSectorPageAnswer<SVM>(SubmitClarificationPageAnswerCommand command,
                                                    Func<Task<SVM>> viewModelBuilder,
                                                    string errorView) where SVM : ClarifierSectorDetailsViewModel
        {
            var validationResponse = await _clarificationPageValidator.Validate(command);

            if (validationResponse.Errors.Any())
            {
                foreach (var error in validationResponse.Errors)
                {
                    ModelState.AddModelError(error.Field, error.ErrorMessage);
                }
            }

            var submittedPageOutcomeSuccessfully = false;

            if (ModelState.IsValid)
            {
                var userId = HttpContext.User.UserId();
                var userName = HttpContext.User.UserDisplayName();

                submittedPageOutcomeSuccessfully = await _clarificationApiClient.SubmitClarificationPageReviewOutcome(command.ApplicationId,
                                    SequenceIds.DeliveringApprenticeshipTraining,
                                    SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees,
                                    command.PageId,
                                    userId,
                                    userName,
                                    command.ClarificationResponse,
                                    command.Status,
                                    command.ReviewComment,
                                    null);

                if (!submittedPageOutcomeSuccessfully)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save outcome as this time");
                }
            }

            if (!submittedPageOutcomeSuccessfully)
            {
                var viewModel = await viewModelBuilder.Invoke();
                viewModel.Status = command.Status;
                viewModel.OptionFailText = command.OptionFailText;
                viewModel.OptionInProgressText = command.OptionInProgressText;
                viewModel.OptionPassText = command.OptionPassText;

                viewModel.ClarificationResponse = command.ClarificationResponse;

                return View(errorView, viewModel);
            }

            return RedirectToAction("ReviewPageAnswers", new
            {
                applicationId = command.ApplicationId,
                sequenceNumber = SequenceIds.DeliveringApprenticeshipTraining,
                sectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees
            });
        }
    }
}