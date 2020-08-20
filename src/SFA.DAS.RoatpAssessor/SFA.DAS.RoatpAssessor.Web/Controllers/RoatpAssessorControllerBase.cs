using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class RoatpAssessorControllerBase<T> : Controller
    {
        protected readonly IRoatpAssessorApiClient _assessorApiClient;
        protected readonly ILogger<T> _logger;
        protected readonly IRoatpAssessorPageValidator AssessorPageValidator;

        public RoatpAssessorControllerBase(IRoatpAssessorApiClient assessorApiClient,
                                           ILogger<T> logger, IRoatpAssessorPageValidator assessorPageValidator)
        {
            _assessorApiClient = assessorApiClient;
            _logger = logger;
            AssessorPageValidator = assessorPageValidator;
        }

        public string SetupGatewayPageOptionTexts(SubmitAssessorPageAnswerCommand command)
        {
            if (command?.Status == null) return string.Empty;
            command.OptionInProgressText = command.Status == AssessorPageReviewStatus.InProgress && !string.IsNullOrEmpty(command.OptionInProgressText) ? command.OptionInProgressText : string.Empty;
            command.OptionPassText = command.Status == AssessorPageReviewStatus.Pass && !string.IsNullOrEmpty(command.OptionPassText) ? command.OptionPassText : string.Empty;
            command.OptionFailText = command.Status == AssessorPageReviewStatus.Fail && !string.IsNullOrEmpty(command.OptionFailText) ? command.OptionFailText : string.Empty;

            switch (command.Status)
            {
                case AssessorPageReviewStatus.Pass:
                    return command.OptionPassText;
                case AssessorPageReviewStatus.Fail:
                    return command.OptionFailText;
                case AssessorPageReviewStatus.InProgress:
                    return command.OptionInProgressText;
                default:
                    return string.Empty;
            }
        }

        protected async Task<IActionResult> ValidateAndUpdatePageAnswer<T>(SubmitAssessorPageAnswerCommand command,
                                                          Func<Task<T>> viewModelBuilder,
                                                          string errorView) where T : ReviewAnswersViewModel
        {
            // TODO: Split function into two actions. One for validating and one for updating page answer
            var validationResponse = await AssessorPageValidator.Validate(command);

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
                var comment = SetupGatewayPageOptionTexts(command);

                submittedPageOutcomeSuccessfully = await _assessorApiClient.SubmitAssessorPageReviewOutcome(command.ApplicationId,
                                    command.SequenceNumber,
                                    command.SectionNumber,
                                    command.PageId,

                                    userId,
                                    command.Status,
                                    comment);

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
                return RedirectToAction("ViewApplication", "Overview", new { applicationId = command.ApplicationId }, $"sequence-{command.SequenceNumber}");
            }
            else
            {
                return RedirectToAction("ReviewPageAnswers", new { applicationId = command.ApplicationId, sequenceNumber = command.SequenceNumber, sectionNumber = command.SectionNumber, pageId = command.NextPageId });
            }
        }



        protected async Task<IActionResult> ValidateAndUpdateSectorPageAnswer<T>(SubmitAssessorPageAnswerCommand command,
                                                    Func<Task<T>> viewModelBuilder,
                                                    string errorView) where T : SectorViewModel
        {
            // TODO: Split function into two actions. One for validating and one for updating page answer
            var validationResponse = await AssessorPageValidator.Validate(command);

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
                var comment = SetupGatewayPageOptionTexts(command);

                submittedPageOutcomeSuccessfully = await _assessorApiClient.SubmitAssessorPageReviewOutcome(command.ApplicationId,
                                    SequenceIds.DeliveringApprenticeshipTraining,
                          SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees,
                                    command.PageId,
                                    userId,
                                    command.Status,
                                    comment);

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