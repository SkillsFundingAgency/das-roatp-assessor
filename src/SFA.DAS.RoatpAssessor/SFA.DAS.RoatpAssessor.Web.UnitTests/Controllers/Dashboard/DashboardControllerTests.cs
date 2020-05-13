using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.UnitTests.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.Dashboard
{
    [TestFixture]
    [Ignore("These tests need to be unignored once staff idams is setup and the temp code in the controllers is removed")]
    public class DashboardControllerTests
    {
        private string _userId;

        private Mock<IAssessorDashboardOrchestrator> _orchestratorMock;
        private Mock<IHttpContextAccessor> _contextAccessorMock;

        private DashboardController _controller;
        
        [SetUp]
        public void Setup()
        {
            _orchestratorMock = new Mock<IAssessorDashboardOrchestrator>();
            _contextAccessorMock = MockedHttpContextAccessor.Setup();
            _controller = new DashboardController(_orchestratorMock.Object, _contextAccessorMock.Object)
            {
                ControllerContext = MockedControllerContext.Setup()
            };

            _userId = _contextAccessorMock.Object.HttpContext.User.UserId();
        }

        [Test]
        public async Task When_getting_new_applications_the_users_applications_are_returned()
        {
            var expectedViewModel = new NewApplicationsViewModel(1, 2, 3, 4);
            _orchestratorMock.Setup(x => x.GetNewApplicationsViewModel(_userId)).ReturnsAsync(expectedViewModel);

            var result = await _controller.NewApplications();

            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task When_getting_in_progress_applications_the_users_applications_are_returned()
        {
            var expectedViewModel = new InProgressApplicationsViewModel(_userId, 1, 2, 3, 4);
            _orchestratorMock.Setup(x => x.GetInProgressApplicationsViewModel(_userId)).ReturnsAsync(expectedViewModel);

            var result = await _controller.InProgressApplications();

            Assert.AreSame(expectedViewModel, result.Model);
        }
    }
}
