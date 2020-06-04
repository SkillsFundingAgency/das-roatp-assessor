﻿using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.AssessorOutcome
{
    [TestFixture]
    public class AssessorOutcomeControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private const AssessorType _assessorType = AssessorType.FirstAssessor;

        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IAssessorOverviewOrchestrator> _assessorOverviewOrchestrator;
        private IRoatpAssessorOutcomeValidator _assessorOutcomeValidator;

        private AssessorOutcomeController _controller;
        private AssessorApplicationViewModel _applicationViewModel;

        [SetUp]
        public void SetUp()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _assessorOutcomeValidator = new RoatpAssessorOutcomeValidator();
            _assessorOverviewOrchestrator = new Mock<IAssessorOverviewOrchestrator>();

            _controller = new AssessorOutcomeController(_applyApiClient.Object, _assessorOverviewOrchestrator.Object, _assessorOutcomeValidator)
            {
                ControllerContext = MockedControllerContext.Setup()
            };

            _applyApiClient.Setup(x => x.UpdateAssessorReviewStatus(_applicationId, (int)_assessorType, _controller.User.UserId(), It.IsAny<string>())).ReturnsAsync(true);

            _applicationViewModel = GetApplicationViewModel();
            _assessorOverviewOrchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetApplicationOverviewRequest>())).ReturnsAsync(_applicationViewModel);
        }

        private AssessorApplicationViewModel GetApplicationViewModel()
        {
            var userId = _controller.User.UserId();
            var application = new Apply { ApplicationId = _applicationId, Assessor1ReviewStatus = AssessorReviewStatus.New, Assessor1UserId = userId };
            var contact = new Contact { Email = userId, GivenNames = _controller.User.GivenName(), FamilyName = _controller.User.Surname() };
            var sequences = new List<AssessorSequence>();

            return new AssessorApplicationViewModel(application, contact, sequences, userId) { AssessorType = _assessorType };
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
            Assert.AreEqual("Overview", result.ControllerName);
            Assert.AreEqual("ViewApplication", result.ActionName);
        }

        [Test]
        public async Task POST_AssessorOutcome_When_MoveToModeration_YES_redirects_to_AssessmentComplete()
        {
            // arrange
            var command = new SubmitAssessorOutcomeCommand { ApplicationId = _applicationId, AssessorType = _assessorType, MoveToModeration = "YES" };

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
            var command = new SubmitAssessorOutcomeCommand { ApplicationId = _applicationId, AssessorType = _assessorType, MoveToModeration = "NO" };

            // act
            var result = await _controller.AssessorOutcome(_applicationId, command) as RedirectToActionResult;

            // assert
            Assert.AreEqual("Overview", result.ControllerName);
            Assert.AreEqual("ViewApplication", result.ActionName);
        }

        [Test]
        public async Task POST_AssessorOutcome_When_MoveToModeration_INVALID_returns_view_with_errors()
        {
            // arrange
            var command = new SubmitAssessorOutcomeCommand { ApplicationId = _applicationId, AssessorType = _assessorType, MoveToModeration = "INVALID" };

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
            Assert.AreEqual("Overview", result.ControllerName);
            Assert.AreEqual("ViewApplication", result.ActionName);
        }
    }
}