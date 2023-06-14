using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.WsFederation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.RoatpAssessor.Web.Settings;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.Account
{
    [TestFixture]
    public class AccountControllerTests
    {
        private AccountController _controller;
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<IWebConfiguration> _mockWebConfiguration;

        [SetUp]
        public void Setup()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockWebConfiguration = new Mock<IWebConfiguration>();
;           _controller = new AccountController(Mock.Of<ILogger<AccountController>>(), _mockConfiguration.Object, _mockWebConfiguration.Object)
            {
                ControllerContext = MockedControllerContext.Setup(),
                Url = Mock.Of<IUrlHelper>()
            };
        }

        [Test]
        public void SignIn_returns_expected_ChallengeResult()
        {
            var result = _controller.SignIn() as ChallengeResult;

            Assert.That(result, Is.Not.Null);
            CollectionAssert.IsNotEmpty(result.AuthenticationSchemes);
            CollectionAssert.Contains(result.AuthenticationSchemes, WsFederationDefaults.AuthenticationScheme);
        }

        [Test]
        public void PostSignIn_redirects_to_Home()
        {
            var result = _controller.PostSignIn() as RedirectToActionResult;

            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
        public void SignOut_returns_expected_SignOutResult()
        {
            var result = _controller.SignOut() as SignOutResult;

            Assert.That(result, Is.Not.Null);
            CollectionAssert.IsNotEmpty(result.AuthenticationSchemes);
            CollectionAssert.Contains(result.AuthenticationSchemes, WsFederationDefaults.AuthenticationScheme);
            CollectionAssert.Contains(result.AuthenticationSchemes, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [Test]
        public void SignedOut_shows_correct_view()
        {
            var result = _controller.SignedOut() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual("SignedOut", result.ViewName);
        }

        [Test]
        public void AccessDenied_shows_correct_view()
        {
            var result = _controller.AccessDenied() as ViewResult;

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual("AccessDenied", result.ViewName);
        }

        [Test]
        public void ChangeSignInDetails_Shows_Correct_View_When_UseGovSignIn_True()
        {
            //arrange
            _mockConfiguration.Setup(x => x["ResourceEnvironmentName"]).Returns("test");
            _mockWebConfiguration.Setup(config => config.UseGovSignIn).Returns(true);

            //sut
            var actual = _controller.ChangeSignInDetails() as ViewResult;

            Assert.That(actual, Is.Not.Null);
            var actualModel = actual?.Model as ChangeSignInDetailsViewModel;
            Assert.AreEqual("https://home.integration.account.gov.uk/settings", actualModel?.SettingsLink);
        }

        [Test]
        public void ChangeSignInDetails_Redirects_to_Home_When_UseGovSignIn_False()
        {
            //arrange
            _mockWebConfiguration.Setup(config => config.UseGovSignIn).Returns(false);

            //sut
            var actual = _controller.ChangeSignInDetails() as RedirectToActionResult;

            //assert
            Assert.That(actual, Is.Not.Null);
            Assert.AreEqual("Home", actual.ControllerName);
            Assert.AreEqual("Dashboard", actual.ActionName);
        }
    }
}
