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

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Moderator
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class ModeratorControllerBase<T> : Controller where T : class
    {
        protected readonly IRoatpModerationApiClient _moderationApiClient;
        protected readonly ILogger<T> _logger;
        protected readonly IModeratorPageValidator _moderatorPageValidator;

        public ModeratorControllerBase(IRoatpModerationApiClient moderationApiClient, ILogger<T> logger, IModeratorPageValidator moderatorPageValidator)
        {
            _moderationApiClient = moderationApiClient;
            _logger = logger;
            _moderatorPageValidator = moderatorPageValidator;
        }

        protected async Task<IActionResult> ValidateAndUpdatePageAnswer<RAVM>(SubmitModeratorPageAnswerCommand command,
                                                          Func<Task<RAVM>> viewModelBuilder,
                                                          string errorView) where RAVM : ModeratorReviewAnswersViewModel
        {
            var validationResponse = await _moderatorPageValidator.Validate(command);

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

                submittedPageOutcomeSuccessfully = await _moderationApiClient.SubmitModeratorPageReviewOutcome(command.ApplicationId,
                                    command.SequenceNumber,
                                    command.SectionNumber,
                                    command.PageId,
                                    userId,
                                    command.Status,
                                    command.ReviewComment,
                                    command.ExternalReviewComment);

                if (!submittedPageOutcomeSuccessfully)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save outcome as this time");
                }
            }

            if (!submittedPageOutcomeSuccessfully)
            {
                var viewModel = await viewModelBuilder.Invoke();
                viewModel.Status = command.Status;
                viewModel.OptionFailExternalText = command.OptionFailExternalText;
                viewModel.OptionFailText = command.OptionFailText;
                viewModel.OptionInProgressText = command.OptionInProgressText;
                viewModel.OptionPassText = command.OptionPassText;

                return View(errorView, viewModel);
            }
            else if (string.IsNullOrEmpty(command.NextPageId))
            {
                return RedirectToAction("ViewApplication", "ModeratorOverview", new { applicationId = command.ApplicationId }, $"sequence-{command.SequenceNumber}");
            }
            else
            {
                return RedirectToAction("ReviewPageAnswers", new { applicationId = command.ApplicationId, sequenceNumber = command.SequenceNumber, sectionNumber = command.SectionNumber, pageId = command.NextPageId });
            }
        }

        protected async Task<IActionResult> ValidateAndUpdateSectorPageAnswer<SVM>(SubmitModeratorPageAnswerCommand command,
                                                    Func<Task<SVM>> viewModelBuilder,
                                                    string errorView) where SVM : ModeratorSectorDetailsViewModel
        {
            var validationResponse = await _moderatorPageValidator.Validate(command);

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

                submittedPageOutcomeSuccessfully = await _moderationApiClient.SubmitModeratorPageReviewOutcome(command.ApplicationId,
                                    SequenceIds.DeliveringApprenticeshipTraining,
                          SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees,
                                    command.PageId,
                                    userId,
                                    command.Status,
                                    command.ReviewComment,
                                    command.ExternalReviewComment);

                if (!submittedPageOutcomeSuccessfully)
                {
                    ModelState.AddModelError(string.Empty, "Unable to save outcome as this time");
                }
            }

            if (!submittedPageOutcomeSuccessfully)
            {
                var viewModel = await viewModelBuilder.Invoke();
                viewModel.Status = command.Status;
                viewModel.OptionFailExternalText = command.OptionFailExternalText;
                viewModel.OptionFailText = command.OptionFailText;
                viewModel.OptionInProgressText = command.OptionInProgressText;
                viewModel.OptionPassText = command.OptionPassText;

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