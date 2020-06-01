﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
    public class DashboardControllerTests
    {
        private Mock<IAssessorDashboardOrchestrator> _assessorOrchestrator;
        private Mock<IModeratorDashboardOrchestrator> _moderatorOrchestrator;

        private DashboardController _controller;
        
        [SetUp]
        public void Setup()
        {
            _assessorOrchestrator = new Mock<IAssessorDashboardOrchestrator>();
            _moderatorOrchestrator = new Mock<IModeratorDashboardOrchestrator>();
            _controller = new DashboardController(_assessorOrchestrator.Object, _moderatorOrchestrator.Object)
            {
                ControllerContext = MockedControllerContext.Setup()
            };
        }

        [Test]
        public async Task Index_redirects_to_new_applications()
        {
            var result = _controller.Index() as RedirectToActionResult;

            Assert.AreEqual("NewApplications", result.ActionName);
        }

        [Test]
        public async Task When_getting_new_applications_the_users_applications_are_returned()
        {
            var userId = _controller.User.UserId();
            var expectedViewModel = new NewApplicationsViewModel(1, 2, 3, 4);
            _assessorOrchestrator.Setup(x => x.GetNewApplicationsViewModel(userId)).ReturnsAsync(expectedViewModel);

            var result = await _controller.NewApplications();

            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task When_getting_in_progress_applications_the_users_applications_are_returned()
        {
            var userId = _controller.User.UserId();
            var expectedViewModel = new InProgressApplicationsViewModel(userId, 1, 2, 3, 4);
            _assessorOrchestrator.Setup(x => x.GetInProgressApplicationsViewModel(userId)).ReturnsAsync(expectedViewModel);

            var result = await _controller.InProgressApplications();

            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task When_getting_in_progress_moderation_the_applications_are_returned()
        {
            var userId = _controller.User.UserId();
            var expectedViewModel = new InModerationApplicationsViewModel(userId, 1, 2, 3, 4);
            _moderatorOrchestrator.Setup(x => x.GetInModerationApplicationsViewModel(userId)).ReturnsAsync(expectedViewModel);

            var result = await _controller.InModerationApplications();

            Assert.AreSame(expectedViewModel, result.Model);
        }

        [Test]
        public async Task When_assigning_to_assessor_then_the_application_is_assigned()
        {
            var userId = _controller.User.UserId();
            var userName = _controller.User.UserDisplayName();
            var applicationId = Guid.NewGuid();
            var assessorNumber = 2;
            
            var result = await _controller.AssignToAssessor(applicationId, assessorNumber) as RedirectToActionResult;

            _assessorOrchestrator.Verify(x => x.AssignApplicationToAssessor(applicationId, assessorNumber, userId, userName));

            Assert.AreEqual("Overview", result.ControllerName);
            Assert.AreEqual("ViewApplication", result.ActionName);
            Assert.AreEqual(applicationId, result.RouteValues["applicationId"]);
        }
    }
}
