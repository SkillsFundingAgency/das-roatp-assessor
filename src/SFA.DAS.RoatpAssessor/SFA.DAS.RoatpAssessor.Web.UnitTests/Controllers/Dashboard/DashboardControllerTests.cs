using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Settings;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.Dashboard
{
    [TestFixture]
    public class DashboardControllerTests
    {
        private Mock<IAssessorDashboardOrchestrator> _orchestratorMock;
        private Mock<IWebConfiguration> _configuration;
        private DashboardController _controller;
        private string _dashboardUrl;

        [SetUp]
        public void Setup()
        {
            _dashboardUrl = "https://dashboard";
            _orchestratorMock = new Mock<IAssessorDashboardOrchestrator>();
            _configuration = new Mock<IWebConfiguration>();
            _configuration.Setup(c => c.EsfaAdminServicesBaseUrl).Returns(_dashboardUrl);

            _controller = new DashboardController(_orchestratorMock.Object, _configuration.Object)
            {
                ControllerContext = MockedControllerContext.Setup()
            };
        }

        [Test]
        public void Index_redirects_to_new_applications()
        {
            var result = _controller.Index() as RedirectToActionResult;

            Assert.AreEqual("NewApplications", result.ActionName);
        }

        [Test]
        public async Task When_getting_new_applications_the_users_applications_are_returned()
        {
            var userId = _controller.User.UserId();
            var expectedViewModel = new NewApplicationsViewModel(1, 2, 3, 4);
            _orchestratorMock.Setup(x => x.GetNewApplicationsViewModel(userId)).ReturnsAsync(expectedViewModel);

            var result = await _controller.NewApplications();

            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task When_getting_in_progress_applications_the_users_applications_are_returned()
        {
            var userId = _controller.User.UserId();
            var expectedViewModel = new InProgressApplicationsViewModel(userId, 1, 2, 3, 4);
            _orchestratorMock.Setup(x => x.GetInProgressApplicationsViewModel(userId)).ReturnsAsync(expectedViewModel);

            var result = await _controller.InProgressApplications();

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

            _orchestratorMock.Verify(x => x.AssignApplicationToAssessor(applicationId, assessorNumber, userId, userName));

            Assert.AreEqual("Overview", result.ControllerName);
            Assert.AreEqual("ViewApplication", result.ActionName);
            Assert.AreEqual(applicationId, result.RouteValues["applicationId"]);
        }
    }
}
