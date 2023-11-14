using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;

using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.Controllers.Assessor;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Extensions;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.AssessorOutcome
{
    [TestFixture]
    public class AssessorOutcomeControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();

        private Mock<IRoatpAssessorApiClient> _assessorApiClient;
        private Mock<IAssessorOverviewOrchestrator> _assessorOverviewOrchestrator;
        private IAssessorOutcomeValidator _assessorOutcomeValidator;

        private AssessorOutcomeController _controller;
        private AssessorApplicationViewModel _applicationViewModel;

        [SetUp]
        public void SetUp()
        {
            _assessorApiClient = new Mock<IRoatpAssessorApiClient>();
            _assessorOutcomeValidator = new AssessorOutcomeValidator();
            _assessorOverviewOrchestrator = new Mock<IAssessorOverviewOrchestrator>();

            _controller = new AssessorOutcomeController(_assessorApiClient.Object, _assessorOverviewOrchestrator.Object, _assessorOutcomeValidator)
            {
                ControllerContext = MockedControllerContext.Setup()
            };

            _assessorApiClient.Setup(x => x.UpdateAssessorReviewStatus(_applicationId, _controller.User.UserId(), _controller.User.UserDisplayName(), It.IsAny<string>())).ReturnsAsync(true);

            _applicationViewModel = GetApplicationViewModel();
            _assessorOverviewOrchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetAssessorOverviewRequest>())).ReturnsAsync(_applicationViewModel);
        }

        private AssessorApplicationViewModel GetApplicationViewModel()
        {
            var userId = _controller.User.UserId();
            var application = new Apply { ApplicationId = _applicationId, Assessor1ReviewStatus = AssessorReviewStatus.New, Assessor1UserId = userId };
            var contact = new Contact { Email = userId, GivenNames = _controller.User.GivenName(), FamilyName = _controller.User.Surname() };
            var sequences = new List<AssessorSequence>();

            return new AssessorApplicationViewModel(application, contact, sequences, userId);
        }

        [Test]
        public async Task AssessorOutcome_When_IsReadyForModeration_TRUE_returns_view_with_expected_viewmodel()
        {
            // arrange
            _applicationViewModel.IsReadyForModeration = true;

            // act
            var result = await _controller.AssessorOutcome(_applicationId) as ViewResult;
            var actualViewModel = result?.Model as AssessorApplicationViewModel;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(_applicationViewModel));
        }

        [Test]
        public async Task AssessorOutcome_When_IsReadyForModeration_FALSE_redirects_to_Application_Overview()
        {
            // arrange
            _applicationViewModel.IsReadyForModeration = false;

            // act
            var result = await _controller.AssessorOutcome(_applicationId) as RedirectToActionResult;

            // assert
            Assert.AreEqual("AssessorOverview", result.ControllerName);
            Assert.AreEqual("ViewApplication", result.ActionName);
        }

        [Test]
        public async Task AssessorOutcome_When_IsAssessorApproved_redirects_to_Home()
        {
            // arrange
            _applicationViewModel.IsAssessorApproved = true;

            // act
            var result = await _controller.AssessorOutcome(_applicationId) as RedirectToActionResult;

            // assert
            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public async Task POST_AssessorOutcome_When_MoveToModeration_YES_redirects_to_AssessmentComplete()
        {
            // arrange
            var command = new SubmitAssessorOutcomeCommand { ApplicationId = _applicationId, MoveToModeration = "YES" };

            // act
            var result = await _controller.AssessorOutcome(_applicationId, command) as RedirectToActionResult;

            // assert
            Assert.AreEqual("AssessorOutcome", result.ControllerName);
            Assert.AreEqual("AssessmentComplete", result.ActionName);
        }

        [Test]
        public async Task POST_AssessorOutcome_When_MoveToModeration_NO_redirects_to_Application_Overview()
        {
            // arrange
            var command = new SubmitAssessorOutcomeCommand { ApplicationId = _applicationId, MoveToModeration = "NO" };

            // act
            var result = await _controller.AssessorOutcome(_applicationId, command) as RedirectToActionResult;

            // assert
            Assert.AreEqual("AssessorOverview", result.ControllerName);
            Assert.AreEqual("ViewApplication", result.ActionName);
        }

        [Test]
        public async Task POST_AssessorOutcome_When_MoveToModeration_INVALID_returns_view_with_errors()
        {
            // arrange
            var command = new SubmitAssessorOutcomeCommand { ApplicationId = _applicationId, MoveToModeration = "INVALID" };

            // act
            var result = await _controller.AssessorOutcome(_applicationId, command) as ViewResult;
            var actualViewModel = result?.Model as AssessorApplicationViewModel;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(_controller.ModelState.IsValid, Is.False);
            Assert.That(_controller.ModelState.ErrorCount, Is.AtLeast(1));
        }

        [Test]
        public async Task AssessmentComplete_When_IsAssessorApproved_TRUE_returns_view_with_expected_viewmodel()
        {
            // arrange
            _applicationViewModel.AssessorReviewStatus = AssessorReviewStatus.Approved;
            _applicationViewModel.IsAssessorApproved = true;

            // act
            var result = await _controller.AssessmentComplete(_applicationId) as ViewResult;
            var actualViewModel = result?.Model as AssessorApplicationViewModel;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(_applicationViewModel));
        }

        [Test]
        public async Task AssessmentComplete_When_IsAssessorApproved_FALSE_redirects_to_Application_Overview()
        {
            // arrange
            _applicationViewModel.AssessorReviewStatus = AssessorReviewStatus.New;
            _applicationViewModel.IsAssessorApproved = false;

            // act
            var result = await _controller.AssessmentComplete(_applicationId) as RedirectToActionResult;

            // assert
            Assert.AreEqual("AssessorOverview", result.ControllerName);
            Assert.AreEqual("ViewApplication", result.ActionName);
        }
    }
}
