using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.Overview
{
    [TestFixture]
    public class OverviewControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();

        private Mock<IAssessorOverviewOrchestrator> _assessorOverviewOrchestrator;
        private OverviewController _controller;
        private AssessorApplicationViewModel _applicationViewModel;

        [SetUp]
        public void SetUp()
        {
            _assessorOverviewOrchestrator = new Mock<IAssessorOverviewOrchestrator>();

            _controller = new OverviewController(_assessorOverviewOrchestrator.Object)
            {
                ControllerContext = MockedControllerContext.Setup()
            };

            _applicationViewModel = GetApplicationViewModel();
            _assessorOverviewOrchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetApplicationOverviewRequest>())).ReturnsAsync(_applicationViewModel);
        }

        private AssessorApplicationViewModel GetApplicationViewModel()
        {
            var userId = _controller.User.UserId();
            var application = new Apply { ApplicationId = _applicationId, Assessor1ReviewStatus = AssessorReviewStatus.New, Assessor1UserId = userId };
            var contact = new Contact { Email = userId, GivenNames = _controller.User.GivenName(), FamilyName = _controller.User.Surname() };
            var sequences = new List<AssessorSequence>();

            return new AssessorApplicationViewModel(application, contact, sequences, userId) { AssessorType = AssessorType.FirstAssessor };
        }

        [Test]
        public async Task ViewApplication_returns_view_with_expected_viewmodel()
        {
            // arrange
            _applicationViewModel.AssessorReviewStatus = AssessorReviewStatus.New;
            _applicationViewModel.IsAssessorApproved = false;

            // act
            var result = await _controller.ViewApplication(_applicationId) as ViewResult;
            var actualViewModel = result?.Model as AssessorApplicationViewModel;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(_applicationViewModel));
        }

        [Test]
        public async Task ViewApplication_when_application_has_been_assessed_redirects_to_AssessmentComplete()
        {
            // arrange
            _applicationViewModel.AssessorReviewStatus = AssessorReviewStatus.Approved;
            _applicationViewModel.IsAssessorApproved = true;

            // act
            var result = await _controller.ViewApplication(_applicationId) as RedirectToActionResult;

            // assert
            Assert.AreEqual("AssessorOutcome", result.ControllerName);
            Assert.AreEqual("AssessmentComplete", result.ActionName);
        }
    }
}
