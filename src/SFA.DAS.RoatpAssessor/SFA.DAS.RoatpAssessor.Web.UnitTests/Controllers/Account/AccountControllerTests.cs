using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.RoatpAssessor.Web.Settings;
using SFA.DAS.RoatpAssessor.Web.UnitTests.Extensions;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.Account;

[TestFixture]
public class AccountControllerTests
{
    private AccountController _controller;
    private Mock<IWebConfiguration> _configurationMock;

    [SetUp]
    public void Setup()
    {
        _configurationMock = new Mock<IWebConfiguration>();
        _configurationMock.Setup(x => x.DfESignInServiceHelpUrl).Returns("test");
        _controller = new AccountController(Mock.Of<ILogger<AccountController>>(), _configurationMock.Object)
        {
            Url = Mock.Of<IUrlHelper>()
        };
        _controller.AddDefaultContextWithUser();
    }

    [Test]
    public void SignIn_returns_expected_ChallengeResult_DfeSignIn()
    {
        var result = _controller.SignIn() as ChallengeResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.AuthenticationSchemes, Is.Not.Empty);
        Assert.That(result.AuthenticationSchemes, Contains.Item(OpenIdConnectDefaults.AuthenticationScheme));
    }

    [Test]
    public void PostSignIn_redirects_to_Home()
    {
        var result = _controller.PostSignIn() as RedirectToActionResult;

        Assert.AreEqual("Home", result.ControllerName);
        Assert.AreEqual("Index", result.ActionName);
    }

    [Test]
    public void SignOut_returns_expected_SignOutResult_For_DfeSignIn()
    {
        var result = _controller.SignOut() as SignOutResult;

        Assert.That(result, Is.Not.Null);
        Assert.That(result.AuthenticationSchemes, Is.Not.Empty);
        Assert.That(result.AuthenticationSchemes, Contains.Item(OpenIdConnectDefaults.AuthenticationScheme));
        Assert.That(result.AuthenticationSchemes, Contains.Item(CookieAuthenticationDefaults.AuthenticationScheme));
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
        var actualModel = result.Model as Error403ViewModel;
        Assert.NotNull(actualModel);
        Assert.AreEqual("test", actualModel.HelpPageLink);
    }
}
