using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.Controllers.Outcome;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.OutcomeOverview
{
    [TestFixture]
    public class OutcomeOverviewControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();

        private Mock<IOutcomeOverviewOrchestrator> _orchestrator;
        private OutcomeOverviewController _controller;

        [SetUp]
        public void SetUp()
        {
            _orchestrator = new Mock<IOutcomeOverviewOrchestrator>();

            _controller = new OutcomeOverviewController(_orchestrator.Object)
            {
                ControllerContext = MockedControllerContext.Setup()
            };
        }

        private OutcomeApplicationViewModel GetApplicationViewModel(string applicationStatus, string moderationStatus)
        {
            var userId = _controller.User.UserId();
            var userDisplayName = _controller.User.UserDisplayName();

            var assessor2Id = $"{ userId }-2";
            var assessor2DisplayName = $"{ userDisplayName }-2";

            var application = new Apply {   
                                            ApplicationId = _applicationId,
                                            ApplicationStatus = applicationStatus,
                                            Assessor1ReviewStatus = AssessorReviewStatus.Approved, Assessor1UserId = userId, Assessor1Name = userDisplayName,
                                            Assessor2ReviewStatus = AssessorReviewStatus.Approved, Assessor2UserId = assessor2Id, Assessor2Name = assessor2DisplayName,
                                            ModerationStatus = moderationStatus,
                                            ApplyData =  new ApplyData
                                            {
                                                ModeratorReviewDetails = new ModeratorReviewDetails
                                                {
                                                    ModeratorUserId = userId,
                                                    ModeratorName = userDisplayName,
                                                    ModeratorComments = null,
                                                    OutcomeDateTime = DateTime.UtcNow
                                                }
                                            }
            };

            var contact = new Contact { Email = userId, GivenNames = _controller.User.GivenName(), FamilyName = _controller.User.Surname() };
            var sequences = new List<ClarificationSequence>();

            return new OutcomeApplicationViewModel(application, contact, sequences);
        }

        [Test]
        public async Task ViewApplication_returns_view_with_expected_viewmodel()
        {
            var _applicationViewModel = GetApplicationViewModel(ApplicationStatus.GatewayAssessed, ModerationStatus.Pass);
            _orchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetOutcomeOverviewRequest>())).ReturnsAsync(_applicationViewModel);

            var result = await _controller.ViewApplication(_applicationId) as ViewResult;
            var actualViewModel = result?.Model as OutcomeApplicationViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(_applicationViewModel));
        }

        [Test]
        public async Task ViewApplication_when_application_is_absent()
        {
            _orchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetOutcomeOverviewRequest>())).ReturnsAsync((OutcomeApplicationViewModel)null);

            var result = await _controller.ViewApplication(_applicationId) as RedirectToActionResult;

            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Index", result.ActionName);
        }

        [TestCase(ModerationStatus.Pass)]
        [TestCase(ModerationStatus.Fail)]
        public async Task ViewApplication_when_Application_Active_shows_expected_view(string moderationStatus)
        {
            var _applicationViewModel = GetApplicationViewModel(ApplicationStatus.GatewayAssessed, moderationStatus);
            _orchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetOutcomeOverviewRequest>())).ReturnsAsync(_applicationViewModel);

            var result = await _controller.ViewApplication(_applicationId) as ViewResult;

            Assert.IsTrue(result.ViewName.EndsWith("Application.cshtml"));
        }

        [Test]
        public async Task ViewApplication_when_Application_Withdrawn_shows_expected_view()
        {
            var _applicationViewModel = GetApplicationViewModel(ApplicationStatus.Withdrawn, ModerationStatus.InProgress);
            _orchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetOutcomeOverviewRequest>())).ReturnsAsync(_applicationViewModel);

            var result = await _controller.ViewApplication(_applicationId) as ViewResult;

            Assert.IsTrue(result.ViewName.EndsWith("Application_Closed.cshtml"));
        }

        [Test]
        public async Task ViewApplication_when_Application_Removed_shows_expected_view()
        {
            var _applicationViewModel = GetApplicationViewModel(ApplicationStatus.Removed, ModerationStatus.InProgress);
            _orchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetOutcomeOverviewRequest>())).ReturnsAsync(_applicationViewModel);

            var result = await _controller.ViewApplication(_applicationId) as ViewResult;

            Assert.IsTrue(result.ViewName.EndsWith("Application_Closed.cshtml"));
        }
    }
}
