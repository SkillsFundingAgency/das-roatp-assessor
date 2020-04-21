using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.RoatpAssessor.Web.UnitTests.MockedObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.Ping
{
    [TestFixture]
    public class PingControllerTests
    {

        private PingController _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new PingController()
            {
                ControllerContext = MockedControllerContext.Setup()                
            };
        }

        [Test]
        public void Ping_returns_Pong()
        {
            var expectedResponse = "Pong";

            var result = _controller.Ping() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(expectedResponse));
        }
    }
}
