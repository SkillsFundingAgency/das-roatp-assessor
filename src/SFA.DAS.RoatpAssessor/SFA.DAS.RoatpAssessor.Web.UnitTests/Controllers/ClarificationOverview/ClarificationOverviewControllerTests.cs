using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Controllers.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.ClarificationOverview
{
    [TestFixture]
    public class ClarificationOverviewControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();

        private Mock<IClarificationOverviewOrchestrator> _overviewOrchestrator;
        private ClarificationOverviewController _controller;
        private ClarifierApplicationViewModel _applicationViewModel;

        [SetUp]
        public void SetUp()
        {
            _overviewOrchestrator = new Mock<IClarificationOverviewOrchestrator>();

            _controller = new ClarificationOverviewController(_overviewOrchestrator.Object)
            {
                ControllerContext = MockedControllerContext.Setup()
            };

            _applicationViewModel = GetApplicationViewModel();
            _overviewOrchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetClarificationOverviewRequest>())).ReturnsAsync(_applicationViewModel);
        }

        private ClarifierApplicationViewModel GetApplicationViewModel()
        {
            var userId = _controller.User.UserId();
            var userDisplayName = _controller.User.UserDisplayName();

            var assessor2Id = $"{ userId }-2";
            var assessor2DisplayName = $"{ userDisplayName }-2";

            var application = new Apply
            {
                ApplicationId = _applicationId,
                Assessor1ReviewStatus = AssessorReviewStatus.Approved,
                Assessor1UserId = userId,
                Assessor1Name = userDisplayName,
                Assessor2ReviewStatus = AssessorReviewStatus.Approved,
                Assessor2UserId = assessor2Id,
                Assessor2Name = assessor2DisplayName,
                ApplyData = new ApplyData
                {
                    ModeratorReviewDetails = new ModeratorReviewDetails
                    {
                        ClarificationRequestedOn = DateTime.Now,
                        ModeratorUserId =  userId,
                        ModeratorName = userDisplayName
                    }
                }
            };

            var contact = new Contact { Email = userId, GivenNames = _controller.User.GivenName(), FamilyName = _controller.User.Surname() };
            var sequences = new List<ClarificationSequence>();

            return new ClarifierApplicationViewModel(application, contact, sequences, userId);
        }

        [TestCase(ModerationStatus.ClarificationSent)]
        [TestCase(ModerationStatus.ClarificationInProgress)]
        public async Task ViewApplication_returns_view_with_expected_viewmodel(string moderationStatus)
        {
            // arrange
            _applicationViewModel.ModerationStatus = moderationStatus;
            _applicationViewModel.IsReadyForClarificationConfirmation = false;

            // act
            var result = await _controller.ViewApplication(_applicationId) as ViewResult;
            var actualViewModel = result?.Model as ClarifierApplicationViewModel;

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
            Assert.AreEqual("ClarificationOutcome", result.ControllerName);
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
            Assert.AreEqual("ClarificationOutcome", result.ControllerName);
            Assert.AreEqual("AssessmentComplete", result.ActionName);
        }
    }
}
