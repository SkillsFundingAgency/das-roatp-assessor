using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using SFA.DAS.RoatpAssessor.Web.Controllers.Clarification;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.ClarificationOutcome
{

    [TestFixture]
    public class ClarificationOutcomeControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private ClarificationOutcomeViewModel _outcomeViewModel;

        private Mock<IClarificationOutcomeOrchestrator> _mockOrchestrator;
        private Mock<IClarificationOutcomeValidator> _mockValidator;
        private Mock<IRoatpModerationApiClient> _mockModerationApiClient;
        private ClarificationOutcomeController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockOrchestrator = new Mock<IClarificationOutcomeOrchestrator>();
            _mockValidator = new Mock<IClarificationOutcomeValidator>();
            _mockModerationApiClient = new Mock<IRoatpModerationApiClient>();
            _controller = new ClarificationOutcomeController(_mockOrchestrator.Object, _mockValidator.Object, _mockModerationApiClient.Object, Mock.Of<ILogger<ClarificationOutcomeController>>())
            {
                ControllerContext = MockedControllerContext.Setup()
            };

            _outcomeViewModel = GetOutcomeViewModel();
            _mockOrchestrator.Setup(x => x.GetClarificationOutcomeViewModel(It.IsAny<GetClarificationOutcomeRequest>()))
                .ReturnsAsync(_outcomeViewModel);
        }

        [Test]
        public async Task ViewOutcome_returns_view_with_expected_viewmodel()
        {
            // arrange
            _outcomeViewModel.ModerationStatus = ModerationStatus.InProgress;

            // act
            var result = await _controller.ViewOutcome(_applicationId) as ViewResult;
            var actualViewModel = result?.Model as ClarificationOutcomeViewModel;

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
            Assert.AreEqual("ClarificationOutcome", result.ControllerName);
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
            Assert.AreEqual("ClarificationOutcome", result.ControllerName);
            Assert.AreEqual("AssessmentComplete", result.ActionName);
        }

        [Test]
        public async Task ViewOutcome_when_application_has_been_picked_that_is_not_completed()
        {
            // arrange
            _outcomeViewModel.ModerationStatus = ModerationStatus.InProgress;
            _mockOrchestrator.Setup(x => x.GetClarificationOutcomeViewModel(It.IsAny<GetClarificationOutcomeRequest>()))
                .ReturnsAsync((ClarificationOutcomeViewModel)null);
            // act
            var result = await _controller.ViewOutcome(_applicationId) as RedirectToActionResult;

            // assert
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public async Task ReviewOutcome_redirect_back_to_outome_when_error()
        {
            var command = new SubmitClarificationOutcomeCommand();
            _mockValidator.Setup(x => x.Validate(command))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>
                    {
                        new ValidationErrorDetail {Field="Status", ErrorMessage = "error"}

                    }
                }
                );

            var result = await _controller.SubmitClarificationOutcome(_applicationId, command) as ViewResult;
            Assert.That(result.Model, Is.SameAs(_outcomeViewModel));
            _mockOrchestrator.Verify(x => x.GetClarificationOutcomeViewModel(It.IsAny<GetClarificationOutcomeRequest>()), Times.Once);
        }

        [Test]
        public async Task ReviewOutcome_goes_to_are_you_sure_when_no_error()
        {
            var command = new SubmitClarificationOutcomeCommand();
            _mockValidator.Setup(x => x.Validate(command))
                .ReturnsAsync(new ValidationResponse
                {
                    Errors = new List<ValidationErrorDetail>()
                }
                );

            var outcomeReviewViewModel = new ClarificationOutcomeReviewViewModel();
            _mockOrchestrator
                .Setup(x => x.GetClarificationOutcomeReviewViewModel(It.IsAny<ReviewClarificationOutcomeRequest>()))
                .ReturnsAsync(outcomeReviewViewModel);
            var result = await _controller.SubmitClarificationOutcome(_applicationId, command) as ViewResult;
            Assert.That(result.Model, Is.SameAs(outcomeReviewViewModel));
            _mockOrchestrator.Verify(x => x.GetClarificationOutcomeReviewViewModel(It.IsAny<ReviewClarificationOutcomeRequest>()), Times.Once);
        }

        private ClarificationOutcomeViewModel GetOutcomeViewModel()
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

            var outcomes = new List<ClarificationPageReviewOutcome>();

            return new ClarificationOutcomeViewModel(application, outcomes);
        }
    }
}