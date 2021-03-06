﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using SFA.DAS.RoatpAssessor.Web.Controllers.Moderator;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.ModeratorOutcome
{

    [TestFixture]
    public class ModeratorOutcomeControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private ModeratorOutcomeViewModel _outcomeViewModel;

        private Mock<IModeratorOutcomeOrchestrator> _mockOrchestrator;
        private Mock<IModeratorOutcomeValidator> _mockValidator;
        private Mock<IRoatpModerationApiClient> _mockModerationApiClient;
        private ModeratorOutcomeController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockOrchestrator = new Mock<IModeratorOutcomeOrchestrator>();
            _mockValidator = new Mock<IModeratorOutcomeValidator>();
            _mockModerationApiClient = new Mock<IRoatpModerationApiClient>();
            _controller = new ModeratorOutcomeController(_mockOrchestrator.Object, _mockValidator.Object, _mockModerationApiClient.Object, Mock.Of<ILogger<ModeratorOutcomeController>>())
            {
                ControllerContext = MockedControllerContext.Setup()
            };

            _outcomeViewModel = GetOutcomeViewModel();
            _mockOrchestrator.Setup(x => x.GetInModerationOutcomeViewModel(It.IsAny<GetModeratorOutcomeRequest>()))
                .ReturnsAsync(_outcomeViewModel);
        }

        [Test]
        public async Task ViewOutcome_returns_view_with_expected_viewmodel()
        {
            // arrange
            _outcomeViewModel.ModerationStatus = ModerationStatus.InProgress;

            // act
            var result = await _controller.ViewOutcome(_applicationId) as ViewResult;
            var actualViewModel = result?.Model as ModeratorOutcomeViewModel;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(_outcomeViewModel));
        }

        [Test]
        public async Task ViewOutcome_when_application_has_been_picked_that_is_pass()
        {
            // arrange
            _outcomeViewModel.ModerationStatus = ModerationStatus.Pass;

            // act
            var result = await _controller.ViewOutcome(_applicationId) as RedirectToActionResult;

            // assert
            Assert.AreEqual("ModeratorOutcome", result.ControllerName);
            Assert.AreEqual("AssessmentComplete", result.ActionName);
        }

        [Test]
        public async Task ViewOutcome_when_application_has_been_picked_that_is_fail()
        {
            // arrange
            _outcomeViewModel.ModerationStatus = ModerationStatus.Fail;

            // act
            var result = await _controller.ViewOutcome(_applicationId) as RedirectToActionResult;

            // assert
            Assert.AreEqual("ModeratorOutcome", result.ControllerName);
            Assert.AreEqual("AssessmentComplete", result.ActionName);
        }

        [Test]
        public async Task ViewOutcome_when_application_has_been_picked_that_is_not_completed()
        {
            // arrange
            _outcomeViewModel.ModerationStatus = ModerationStatus.InProgress;
            _mockOrchestrator.Setup(x => x.GetInModerationOutcomeViewModel(It.IsAny<GetModeratorOutcomeRequest>()))
                .ReturnsAsync((ModeratorOutcomeViewModel)null);
            // act
            var result = await _controller.ViewOutcome(_applicationId) as RedirectToActionResult;

            // assert
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public async Task ReviewOutcome_redirect_back_to_outome_when_error()
        {
            var command = new SubmitModeratorOutcomeCommand();
            _mockValidator.Setup(x=>x.Validate(command))
                .ReturnsAsync(new ValidationResponse { Errors= new List<ValidationErrorDetail>
                    {
                        new ValidationErrorDetail {Field="Status", ErrorMessage = "error"}

                    }}
                );

            var result = await _controller.SubmitModeratorOutcome(_applicationId, command) as ViewResult;
            Assert.That(result.Model, Is.SameAs(_outcomeViewModel));
            _mockOrchestrator.Verify(x=>x.GetInModerationOutcomeViewModel(It.IsAny<GetModeratorOutcomeRequest>()),Times.Once);
        }

        [Test]
        public async Task ReviewOutcome_goes_to_are_you_sure_when_no_error()
        {
            var command = new SubmitModeratorOutcomeCommand();
            _mockValidator.Setup(x => x.Validate(command))
                .ReturnsAsync(new ValidationResponse
                    {
                        Errors = new List<ValidationErrorDetail>()
                    }
                );

            var outcomeReviewViewModel = new ModeratorOutcomeReviewViewModel();
            _mockOrchestrator
                .Setup(x => x.GetInModerationOutcomeReviewViewModel(It.IsAny<ReviewModeratorOutcomeRequest>()))
                .ReturnsAsync(outcomeReviewViewModel);
            var result = await _controller.SubmitModeratorOutcome(_applicationId, command) as ViewResult;
            Assert.That(result.Model, Is.SameAs(outcomeReviewViewModel));
            _mockOrchestrator.Verify(x => x.GetInModerationOutcomeReviewViewModel(It.IsAny<ReviewModeratorOutcomeRequest>()), Times.Once);
        }


        [Test]
        public async Task Outcome_confirmation_redirect_back_to_outcome_when_error()
        {
            var command = new SubmitModeratorOutcomeConfirmationCommand("","");
            _mockValidator.Setup(x => x.Validate(command))
                .ReturnsAsync(new ValidationResponse
                    {
                        Errors = new List<ValidationErrorDetail>
                        {
                            new ValidationErrorDetail {Field="Status", ErrorMessage = "error"}

                        }
                    }
                );

            var result = await _controller.SubmitModeratorOutcomeConfirmation(_applicationId,string.Empty,command) as ViewResult;
            Assert.That(result.Model, Is.SameAs(_outcomeViewModel));
            _mockOrchestrator.Verify(x => x.GetInModerationOutcomeViewModel(It.IsAny<GetModeratorOutcomeRequest>()), Times.Once);
        }

        [Test]
        public async Task Outcome_confirmation_redirect_back_to_application_when_status_is_pass_and_confirm_status_is_No()
        {
            var command = new SubmitModeratorOutcomeConfirmationCommand(ModerationConfirmationStatus.Pass, "No");
            _mockValidator.Setup(x => x.Validate(command))
                .ReturnsAsync(new ValidationResponse()
                );

            var reviewComment = "comment goes here";
            var result = await _controller.SubmitModeratorOutcomeConfirmation(_applicationId, reviewComment, command) as ViewResult;
            _outcomeViewModel.OptionPassText = reviewComment;
            Assert.That(result.Model, Is.SameAs(_outcomeViewModel));
            _mockOrchestrator.Verify(x => x.GetInModerationOutcomeViewModel(It.IsAny<GetModeratorOutcomeRequest>()), Times.Once);
        }

        [Test]
        public async Task Outcome_confirmation_redirect_back_to_application_when_status_is_fail_and_confirm_status_is_No()
        {
            var command = new SubmitModeratorOutcomeConfirmationCommand(ModerationConfirmationStatus.Fail, "No");
            _mockValidator.Setup(x => x.Validate(command))
                .ReturnsAsync(new ValidationResponse()
                );

            var reviewComment = "comment goes here";
            var result = await _controller.SubmitModeratorOutcomeConfirmation(_applicationId, reviewComment, command) as ViewResult;
            _outcomeViewModel.OptionFailText = reviewComment;
            Assert.That(result.Model, Is.SameAs(_outcomeViewModel));
            _mockOrchestrator.Verify(x => x.GetInModerationOutcomeViewModel(It.IsAny<GetModeratorOutcomeRequest>()), Times.Once);
        }

        [Test]
        public async Task Outcome_confirmation_redirect_back_to_application_when_status_is_ask_for_clarification_and_confirm_status_is_No()
        {
            var command = new SubmitModeratorOutcomeConfirmationCommand(ModerationConfirmationStatus.AskForClarification, "No");
            _mockValidator.Setup(x => x.Validate(command))
                .ReturnsAsync(new ValidationResponse()
                );

            var reviewComment = "comment goes here";
            var result = await _controller.SubmitModeratorOutcomeConfirmation(_applicationId, reviewComment, command) as ViewResult;
            _outcomeViewModel.OptionAskForClarificationText = reviewComment;
            Assert.That(result.Model, Is.SameAs(_outcomeViewModel));
            _mockOrchestrator.Verify(x => x.GetInModerationOutcomeViewModel(It.IsAny<GetModeratorOutcomeRequest>()), Times.Once);
        }

        [Test]
        public async Task Outcome_confirmation_redirect_back_to_application_when_save_is_unsuccessful()
        {
            var command = new SubmitModeratorOutcomeConfirmationCommand(string.Empty, "Yes");
            _mockValidator.Setup(x => x.Validate(command))
                .ReturnsAsync(new ValidationResponse()
                );


            _mockModerationApiClient.Setup(x => x.SubmitModerationOutcome(_applicationId, It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(false);

            var result = await _controller.SubmitModeratorOutcomeConfirmation(_applicationId, string.Empty, command) as ViewResult;
            Assert.That(result.Model, Is.SameAs(_outcomeViewModel));
            _mockOrchestrator.Verify(x => x.GetInModerationOutcomeViewModel(It.IsAny<GetModeratorOutcomeRequest>()), Times.Once);
        }


        [Test]
        public async Task Outcome_confirmation_directs_to_saved_page_when_save_is_successful()
        {
            var command = new SubmitModeratorOutcomeConfirmationCommand(string.Empty, "Yes");
            _mockValidator.Setup(x => x.Validate(command))
                .ReturnsAsync(new ValidationResponse()
                );

            _mockModerationApiClient.Setup(x => x.SubmitModerationOutcome(_applicationId, It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            var outcomeReviewViewModel = new ModeratorOutcomeReviewViewModel();
            _mockOrchestrator.Setup(x => x.GetInModerationOutcomeReviewViewModel(It.IsAny<ReviewModeratorOutcomeRequest>()))
                .ReturnsAsync(outcomeReviewViewModel);

            var result = await _controller.SubmitModeratorOutcomeConfirmation(_applicationId, string.Empty, command) as ViewResult;
            Assert.That(result.Model, Is.SameAs(outcomeReviewViewModel));
            _mockOrchestrator.Verify(x => x.GetInModerationOutcomeReviewViewModel(It.IsAny<ReviewModeratorOutcomeRequest>()), Times.Once);
        }

        private ModeratorOutcomeViewModel GetOutcomeViewModel()
        {
            var userId = _controller.User.UserId();
            var userDisplayName = _controller.User.UserDisplayName();

            var assessor2Id = $"{userId}-2";
            var assessor2DisplayName = $"{userDisplayName}-2";

            var application = new Apply
            {
                ApplicationId = _applicationId,
                Assessor1ReviewStatus = AssessorReviewStatus.Approved,
                Assessor1UserId = userId,
                Assessor1Name = userDisplayName,
                Assessor2ReviewStatus = AssessorReviewStatus.Approved,
                Assessor2UserId = assessor2Id,
                Assessor2Name = assessor2DisplayName
            };

            var outcomes = new List<ModeratorPageReviewOutcome>();

            return new ModeratorOutcomeViewModel(application, outcomes);
        }
    }
}