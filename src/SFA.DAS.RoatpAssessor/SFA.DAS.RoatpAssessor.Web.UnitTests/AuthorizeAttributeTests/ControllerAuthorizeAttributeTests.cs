using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Controllers;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.AuthorizeAttributeTests
{
    [TestFixture]
    public class ControllerAuthorizeAttributeTests
    {

        private readonly List<string> _controllersThatDoNotRequireAuthorize = new List<string>()
        {
            "PingController",
            "ErrorPageController",
            "HomeController"
        };

        [Test]
        [Ignore("Authorize attributes can't be added until we have staff idams set up")]
        public void ControllersShouldHaveAuthorizeAttribute()
        {
            var webAssembly = typeof(PingController).GetTypeInfo().Assembly;

            var controllers = webAssembly.DefinedTypes.Where(c => c.BaseType == typeof(Controller)).ToList();

            foreach (var controller in controllers.Where(c => !_controllersThatDoNotRequireAuthorize.Contains(c.Name)))
            {
                var hasAuthorize = controller.GetCustomAttributesData().Any(cad => cad.AttributeType == typeof(AuthorizeAttribute) || cad.AttributeType.BaseType == typeof(AuthorizeAttribute));

                if (!hasAuthorize)
                {
                    Assert.Fail($"Controller {controller.Name} is not decorated with AuthorizeAttribute");
                }
            }
        }
    }
}
