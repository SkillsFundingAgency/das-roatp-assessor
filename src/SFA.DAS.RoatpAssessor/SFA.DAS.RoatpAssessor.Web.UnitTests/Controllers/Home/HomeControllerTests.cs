using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.UnitTests.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class HomeControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();

        private Mock<IAssessorOverviewOrchestrator> _assessorOverviewOrchestrator;
        private HomeController _controller;

        [SetUp]
        public void SetUp()
        {
            _assessorOverviewOrchestrator = new Mock<IAssessorOverviewOrchestrator>();

            _controller = new HomeController(_assessorOverviewOrchestrator.Object)
            {
                ControllerContext = MockedControllerContext.Setup()
            };
        }

        [Test]
        public void Error_returns_view_with_expected_viewmodel()
        {
            var expectedViewModel = new ErrorViewModel { RequestId = _controller.HttpContext.TraceIdentifier };

            var result = _controller.Error() as ViewResult;
            var actualViewModel = result?.Model as ErrorViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);           
            Assert.That(actualViewModel.RequestId, Is.EqualTo(expectedViewModel.RequestId));
            Assert.That(actualViewModel.ShowRequestId, Is.EqualTo(!string.IsNullOrEmpty(expectedViewModel.RequestId)));
        }

        [Test]
        public async Task ViewApplication_returns_view_with_expected_viewmodel()
        {
            // arrange
            var application = new Apply { ApplicationId = _applicationId, AssessorReviewStatus = ApplyTypes.AssessorReviewStatus.New };
            var viewModel = new AssessorApplicationViewModel(application);

            var request = new GetApplicationOverviewRequest(_applicationId, _controller.User.UserDisplayName());
            _assessorOverviewOrchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetApplicationOverviewRequest>())).ReturnsAsync(viewModel);

            // act
            var result = await _controller.ViewApplication(_applicationId) as ViewResult;
            var actualViewModel = result?.Model as AssessorApplicationViewModel;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(viewModel));
        }
    }
}
