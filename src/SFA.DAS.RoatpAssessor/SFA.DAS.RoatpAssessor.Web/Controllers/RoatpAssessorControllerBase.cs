using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    //[ExternalApiExceptionFilter]
    //[Authorize(Roles = Roles.RoatpGatewayAssessorTeam)]
    //[FeatureToggle(FeatureToggles.EnableRoatpGatewayReview, "Dashboard", "Index")]
    public class RoatpAssessorControllerBase<T> : Controller
    {
        protected readonly IHttpContextAccessor _contextAccessor;
        protected readonly IRoatpApplicationApiClient _applyApiClient;
        protected readonly ILogger<T> _logger;
        protected readonly IRoatpAssessorPageValidator AssessorPageValidator;
        //protected const string GatewayViewsLocation = "~/Views/Roatp/Apply/Gateway/pages";

        public RoatpAssessorControllerBase()
        {

        }

        public RoatpAssessorControllerBase(IHttpContextAccessor contextAccessor, IRoatpApplicationApiClient applyApiClient,
                                          ILogger<T> logger, IRoatpAssessorPageValidator assessorPageValidator)
        {
            _contextAccessor = contextAccessor;
            _applyApiClient = applyApiClient;
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
            var validationResponse = await AssessorPageValidator.Validate(command);
            if (validationResponse.Errors != null && validationResponse.Errors.Any())
            {
                var viewModel = await viewModelBuilder.Invoke();
                viewModel.Status = command.Status;
                viewModel.OptionFailText = command.OptionFailText;
                viewModel.OptionInProgressText = command.OptionInProgressText;
                viewModel.OptionPassText = command.OptionPassText;
                viewModel.ErrorMessages = validationResponse.Errors;
                return View(errorView, viewModel);
            }
            else
            {
                await SubmitAssessorPageOutcome(command);

                // TODO: Change the logic to return to ApplicationOverview or to next page
                var viewModel = new ReviewAnswersViewModel { ApplicationId = command.ApplicationId };
                return View(errorView, viewModel);
            }
        }

        protected async Task SubmitAssessorPageOutcome(SubmitAssessorPageAnswerCommand command)
        {
            var userId = "4dsfdg-MyGuidUserId-yf6re"; // _contextAccessor.HttpContext.User.UserDisplayName();
            var comment = SetupGatewayPageOptionTexts(command);

            _logger.LogInformation($"{typeof(T).Name}-SubmitAssessorPageOutcome - ApplicationId '{command.ApplicationId}' - " +
                                                    $"SequenceNumber '{command.SequenceNumber}' - SectionNumber '{command.SectionNumber}' - PageId '{command.PageId}' - " +
                                                    $"AssessorType '{command.AssessorType}' - UserId '{userId}' - " +
                                                    $"Status '{command.Status}' - Comment '{comment}'");
            
            try
            {
                await _applyApiClient.SubmitAssessorPageOutcome(command.ApplicationId,
                                                    command.SequenceNumber,
                                                    command.SectionNumber,
                                                    command.PageId,
                                                    (int)command.AssessorType,
                                                    userId,
                                                    command.Status,
                                                    comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{typeof(T).Name}-SubmitAssessorPageOutcome - Error: '" + ex.Message + "'");
                throw;
            }
        }

    }
}