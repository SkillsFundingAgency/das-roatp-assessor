using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.Dashboard
{
    [TestFixture]
    [Ignore("These tests need to be unignored once staff idams is setup and the temp code in the controllers is removed")]
    public class DashboardControllerTests
    {
        private Mock<IAssessorDashboardOrchestrator> _orchestratorMock;
        private Mock<IHttpContextAccessor> _contextAccessorMock;
        private DashboardController _controller;
        private const string ExpectedUserName = "someuser";

        [SetUp]
        public void Setup()
        {
            _orchestratorMock = new Mock<IAssessorDashboardOrchestrator>();
            _contextAccessorMock = new Mock<IHttpContextAccessor>();
            _controller = new DashboardController(_orchestratorMock.Object, _contextAccessorMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", ExpectedUserName),
            }));
            _contextAccessorMock.Setup(x => x.HttpContext).Returns(new DefaultHttpContext { User = user });
        }

        [Test]
        public async Task When_getting_new_applications_the_users_applications_are_returned()
        {
            var expectedViewModel = new NewApplicationsViewModel(1, 2, 3, 4);
            _orchestratorMock.Setup(x => x.GetNewApplicationsViewModel(ExpectedUserName)).ReturnsAsync(expectedViewModel);

            var result = await _controller.NewApplications();

            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task When_getting_in_progress_applications_the_users_applications_are_returned()
        {
            var expectedViewModel = new InProgressApplicationsViewModel(ExpectedUserName, 1, 2, 3, 4);
            _orchestratorMock.Setup(x => x.GetInProgressApplicationsViewModel(ExpectedUserName)).ReturnsAsync(expectedViewModel);

            var result = await _controller.InProgressApplications();

            Assert.AreSame(expectedViewModel, result.Model);
        }
    }
}
