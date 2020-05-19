using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Settings;
using SFA.DAS.RoatpAssessor.Web.UnitTests.MockedObjects;
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
        public void Dashboard_redirects_to_external_dasbhoard_url()
        {
            var result = _controller.Dashboard() as RedirectResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Url, $"{_dashboardUrl}/Dashboard");
        }
    }
}
