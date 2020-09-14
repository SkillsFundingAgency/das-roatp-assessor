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

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Assessor
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class AssessorControllerBase<T> : Controller where T : class
    {
        protected readonly IRoatpAssessorApiClient _assessorApiClient;
        protected readonly ILogger<T> _logger;
        protected readonly IAssessorPageValidator _assessorPageValidator;

        public AssessorControllerBase(IRoatpAssessorApiClient assessorApiClient, ILogger<T> logger, IAssessorPageValidator assessorPageValidator)
        {
            _assessorApiClient = assessorApiClient;
            _logger = logger;
            _assessorPageValidator = assessorPageValidator;
        }

        protected async Task<IActionResult> ValidateAndUpdatePageAnswer<RAVM>(SubmitAssessorPageAnswerCommand command,
                                                          Func<Task<RAVM>> viewModelBuilder,
                                                          string errorView) where RAVM : AssessorReviewAnswersViewModel
        {
            var validationResponse = await _assessorPageValidator.Validate(command);

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

                submittedPageOutcomeSuccessfully = await _assessorApiClient.SubmitAssessorPageReviewOutcome(command.ApplicationId,
                                    command.SequenceNumber,
                                    command.SectionNumber,
                                    command.PageId,
                                    userId,
                                    command.Status,
                                    command.ReviewComment);

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

                return View(errorView, viewModel);
            }
            else if (string.IsNullOrEmpty(command.NextPageId))
            {
                return RedirectToAction("ViewApplication", "AssessorOverview", new { applicationId = command.ApplicationId }, $"sequence-{command.SequenceNumber}");
            }
            else
            {
                return RedirectToAction("ReviewPageAnswers", new { applicationId = command.ApplicationId, sequenceNumber = command.SequenceNumber, sectionNumber = command.SectionNumber, pageId = command.NextPageId });
            }
        }

        protected async Task<IActionResult> ValidateAndUpdateSectorPageAnswer<SVM>(SubmitAssessorPageAnswerCommand command,
                                                    Func<Task<SVM>> viewModelBuilder,
                                                    string errorView) where SVM : AssessorSectorDetailsViewModel
        {
            var validationResponse = await _assessorPageValidator.Validate(command);

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

                submittedPageOutcomeSuccessfully = await _assessorApiClient.SubmitAssessorPageReviewOutcome(command.ApplicationId,
                                    SequenceIds.DeliveringApprenticeshipTraining,
                          SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees,
                                    command.PageId,
                                    userId,
                                    command.Status,
                                    command.ReviewComment);

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