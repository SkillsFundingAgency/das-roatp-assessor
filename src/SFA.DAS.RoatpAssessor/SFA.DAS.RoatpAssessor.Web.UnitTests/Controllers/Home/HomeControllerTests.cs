using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.RoatpAssessor.Web.UnitTests.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.Home
{
    [TestFixture]
    public class HomeControllerTests
    {
        private HomeController _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new HomeController()
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
        public void Index_redirects_to_new_applications_dashboard()
        {
            var result = _controller.Index() as RedirectToActionResult;

            Assert.AreEqual("Dashboard", result.ControllerName);
            Assert.AreEqual("NewApplications", result.ActionName);
        }
    }
}
