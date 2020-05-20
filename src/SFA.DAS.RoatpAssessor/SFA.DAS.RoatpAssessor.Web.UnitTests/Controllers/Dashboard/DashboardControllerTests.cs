using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.Dashboard
{
    [TestFixture]
    public class DashboardControllerTests
    {
        private Mock<IAssessorDashboardOrchestrator> _orchestratorMock;

        private DashboardController _controller;
        
        [SetUp]
        public void Setup()
        {
            _orchestratorMock = new Mock<IAssessorDashboardOrchestrator>();
            _controller = new DashboardController(_orchestratorMock.Object)
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
    }
}
