using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Controllers.Assessor;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Controllers.Clarification;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.ClarificationOverview
{
    [TestFixture]
    public class ClarificationOverviewControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();

        private ClarificationOverviewController _controller;

        [SetUp]
        public void SetUp()
        {
            _controller = new ClarificationOverviewController()
            {
                ControllerContext = MockedControllerContext.Setup()
            };
        }

        [Test]
        public async Task ViewApplication_returns_view_with_expected_viewmodel()
        {
            // arrange

            // act
            var result = await _controller.ViewApplication(_applicationId) as ViewResult;
            var actualViewModel = result?.Model as dynamic;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
        }

    }
}
