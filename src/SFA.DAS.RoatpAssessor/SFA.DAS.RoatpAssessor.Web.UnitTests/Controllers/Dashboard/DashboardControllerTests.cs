using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.Dashboard
{
    [TestFixture]
    public class DashboardControllerTests
    {
        private Mock<IAssessorDashboardOrchestrator> _assessorOrchestrator;
        private Mock<IModeratorDashboardOrchestrator> _moderatorOrchestrator;
        private Mock<IClarificationDashboardOrchestrator> _clarificationOrchestrator;
        private Mock<IOutcomeDashboardOrchestrator> _outcomeOrchestrator;

        private DashboardController _controller;
        private string _dashboardUrl;

        [SetUp]
        public void Setup()
        {
            _dashboardUrl = "https://dashboard";

            _assessorOrchestrator = new Mock<IAssessorDashboardOrchestrator>();
            _moderatorOrchestrator = new Mock<IModeratorDashboardOrchestrator>();
            _clarificationOrchestrator = new Mock<IClarificationDashboardOrchestrator>();
            _outcomeOrchestrator = new Mock<IOutcomeDashboardOrchestrator>();
            _controller = new DashboardController(_assessorOrchestrator.Object, _moderatorOrchestrator.Object, _clarificationOrchestrator.Object, _outcomeOrchestrator.Object)
            {
                ControllerContext = MockedControllerContext.Setup()
            };
        }

        [Test]
        public async Task When_getting_new_applications_the_users_applications_are_returned()
        {
            var userId = _controller.User.UserId();
            var expectedViewModel = new NewApplicationsViewModel(1, 2, 3, 4, 5);
            _assessorOrchestrator.Setup(x => x.GetNewApplicationsViewModel(userId,null,null)).ReturnsAsync(expectedViewModel);

            var result = await _controller.NewApplications(null,null);

            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task When_getting_in_progress_applications_the_users_applications_are_returned()
        {
            var userId = _controller.User.UserId();
            var expectedViewModel = new InProgressApplicationsViewModel(userId, 1, 2, 3, 4, 5);
            _assessorOrchestrator.Setup(x => x.GetInProgressApplicationsViewModel(userId,null,null)).ReturnsAsync(expectedViewModel);

            var result = await _controller.InProgressApplications(null,null);

            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task When_getting_in_moderation_applications_the_applications_are_returned()
        {
            var userId = _controller.User.UserId();
            var expectedViewModel = new InModerationApplicationsViewModel(userId, 1, 2, 3, 4, 5);
            _moderatorOrchestrator.Setup(x => x.GetInModerationApplicationsViewModel(userId)).ReturnsAsync(expectedViewModel);

            var result = await _controller.InModerationApplications();

            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task When_getting_in_clarification_applications_the_applications_are_returned()
        {
            var userId = _controller.User.UserId();
            var expectedViewModel = new InClarificationApplicationsViewModel(userId, 1, 2, 3, 4, 5);
            _clarificationOrchestrator.Setup(x => x.GetInClarificationApplicationsViewModel(userId)).ReturnsAsync(expectedViewModel);

            var result = await _controller.InClarificationApplications();

            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task When_getting_closed_applications_the_applications_are_returned()
        {
            var userId = _controller.User.UserId();
            var expectedViewModel = new ClosedApplicationsViewModel(userId, 1, 2, 3, 4, 5);
            _outcomeOrchestrator.Setup(x => x.GetClosedApplicationsViewModel(userId)).ReturnsAsync(expectedViewModel);

            var result = await _controller.ClosedApplications();

            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task When_assigning_to_assessor_then_the_application_is_assigned()
        {
            var userId = _controller.User.UserId();
            var userName = _controller.User.UserDisplayName();
            var applicationId = Guid.NewGuid();
            var assessorNumber = 2;

            var result = await _controller.AssignToAssessor(applicationId, assessorNumber) as RedirectToActionResult;

            _assessorOrchestrator.Verify(x => x.AssignApplicationToAssessor(applicationId, assessorNumber, userId, userName));

            Assert.AreEqual("AssessorOverview", result.ControllerName);
            Assert.AreEqual("ViewApplication", result.ActionName);
            Assert.AreEqual(applicationId, result.RouteValues["applicationId"]);
        }
    }
}
