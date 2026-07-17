using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Extensions;

public static class ControllerExtensions
{
    private const string GivenName = "Test";
    private const string Surname = "User";
    private const string Email = "Test.User@example.com";
    private const string RoleClaimType = "http://service/service";
    public static Controller AddDefaultContextWithUser(this Controller controller, params string[] roles)
    {
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = GetMockedUser(roles)
            }
        };
        controller.ControllerContext.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>());
        controller.Url = Mock.Of<IUrlHelper>();
        return controller;
    }

    public static ClaimsPrincipal GetMockedUser(params string[] roles)
    {
        List<Claim> claims =
        [
            new (ClaimTypes.GivenName, GivenName),
            new (ClaimTypes.Surname, Surname),
            new (ClaimTypes.Name, $"{GivenName} {Surname}"),
            new (ClaimTypes.Email, Email),
            new (ClaimTypes.Upn, Email)
        ];

        if (roles != null)
        {
            foreach (var role in roles)
            {
                var rolesClaim = new Claim(RoleClaimType, role);
                claims.Add(rolesClaim);
            }
        }
        var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock", ClaimTypes.Name, RoleClaimType));

        return user;
    }
}
