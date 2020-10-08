using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Controllers.Moderator;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.ModeratorOverview
{
    [TestFixture]
    public class ModeratorOverviewControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();

        private Mock<IModeratorOverviewOrchestrator> _moderatorOverviewOrchestrator;
        private ModeratorOverviewController _controller;
        private ModeratorApplicationViewModel _applicationViewModel;

        [SetUp]
        public void SetUp()
        {
            _moderatorOverviewOrchestrator = new Mock<IModeratorOverviewOrchestrator>();

            _controller = new ModeratorOverviewController(_moderatorOverviewOrchestrator.Object)
            {
                ControllerContext = MockedControllerContext.Setup()
            };

            _applicationViewModel = GetApplicationViewModel();
            _moderatorOverviewOrchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetModeratorOverviewRequest>())).ReturnsAsync(_applicationViewModel);
        }

        private ModeratorApplicationViewModel GetApplicationViewModel()
        {
            var userId = _controller.User.UserId();
            var userDisplayName = _controller.User.UserDisplayName();

            var assessor2Id = $"{ userId }-2";
            var assessor2DisplayName = $"{ userDisplayName }-2";

            var application = new Apply {   
                                            ApplicationId = _applicationId,
                                            Assessor1ReviewStatus = AssessorReviewStatus.Approved, Assessor1UserId = userId, Assessor1Name = userDisplayName,
                                            Assessor2ReviewStatus = AssessorReviewStatus.Approved, Assessor2UserId = assessor2Id, Assessor2Name = assessor2DisplayName
            };

            var contact = new Contact { Email = userId, GivenNames = _controller.User.GivenName(), FamilyName = _controller.User.Surname() };
            var sequences = new List<ModeratorSequence>();

            return new ModeratorApplicationViewModel(application, contact, sequences, userId);
        }

        [Test]
        public async Task ViewApplication_returns_view_with_expected_viewmodel()
        {
            // arrange
            _applicationViewModel.ModerationStatus = ModerationStatus.New;
            _applicationViewModel.IsReadyForModeratorConfirmation = false;

            // act
            var result = await _controller.ViewApplication(_applicationId) as ViewResult;
            var actualViewModel = result?.Model as ModeratorApplicationViewModel;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(_applicationViewModel));
        }

        [Test]
        public async Task ViewApplication_when_application_has_been_passed_redirects_to_AssessmentComplete()
        {
            // arrange
            _applicationViewModel.ModerationStatus = ModerationStatus.Pass;

            // act
            var result = await _controller.ViewApplication(_applicationId) as RedirectToActionResult;

            // assert
            Assert.AreEqual("ModeratorOutcome", result.ControllerName);
            Assert.AreEqual("AssessmentComplete", result.ActionName);
        }

        [Test]
        public async Task ViewApplication_when_application_has_been_failed_redirects_to_AssessmentComplete()
        {
            // arrange
            _applicationViewModel.ModerationStatus = ModerationStatus.Fail;

            // act
            var result = await _controller.ViewApplication(_applicationId) as RedirectToActionResult;

            // assert
            Assert.AreEqual("ModeratorOutcome", result.ControllerName);
            Assert.AreEqual("AssessmentComplete", result.ActionName);
        }
    }
}
