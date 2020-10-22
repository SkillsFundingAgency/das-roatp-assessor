using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Controllers.Moderator;
using SFA.DAS.RoatpAssessor.Web.Controllers.Outcome;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.OutcomeOverview
{
    [TestFixture]
    public class OutcomeOverviewControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();

        private Mock<IOutcomeOverviewOrchestrator> _orchestrator;
        private OutcomeOverviewController _controller;
        private OutcomeApplicationViewModel _applicationViewModel;
        private readonly string _moderatorName = "James Smith";
        private readonly string _moderationStatus = "Pass";
        private DateTime _outcomeDate = DateTime.UtcNow;

            [SetUp]
        public void SetUp()
        {
            _orchestrator = new Mock<IOutcomeOverviewOrchestrator>();

            _controller = new OutcomeOverviewController(_orchestrator.Object)
            {
                ControllerContext = MockedControllerContext.Setup()
            };

            _applicationViewModel = GetApplicationViewModel();
            _orchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetOutcomeOverviewRequest>())).ReturnsAsync(_applicationViewModel);
        }

        private OutcomeApplicationViewModel GetApplicationViewModel()
        {
            var userId = _controller.User.UserId();
            var userDisplayName = _controller.User.UserDisplayName();

            var assessor2Id = $"{ userId }-2";
            var assessor2DisplayName = $"{ userDisplayName }-2";

            var application = new Apply {   
                                            ApplicationId = _applicationId,
                                            Assessor1ReviewStatus = AssessorReviewStatus.Approved, Assessor1UserId = userId, Assessor1Name = userDisplayName,
                                            Assessor2ReviewStatus = AssessorReviewStatus.Approved, Assessor2UserId = assessor2Id, Assessor2Name = assessor2DisplayName,
                                            ModerationStatus = _moderationStatus,
                                            ApplyData =  new ApplyData
                                            {
                                                ModeratorReviewDetails = new ModeratorReviewDetails
                                                {
                                                    ModeratorName = _moderatorName,
                                                    OutcomeDateTime = _outcomeDate
                                                }
                                            }
            };

            var contact = new Contact { Email = userId, GivenNames = _controller.User.GivenName(), FamilyName = _controller.User.Surname() };
            var sequences = new List<ModeratorSequence>();

            return new OutcomeApplicationViewModel(application, contact, sequences, userId);
        }

        [Test]
        public async Task ViewApplication_returns_view_with_expected_viewmodel()
        {
            var result = await _controller.ViewOutcome(_applicationId) as ViewResult;
            var actualViewModel = result?.Model as OutcomeApplicationViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(_applicationViewModel));
        }

        [Test]
        public async Task ViewApplication_when_application_is_absent()
        {
            _orchestrator.Setup(x => x.GetOverviewViewModel(It.IsAny<GetOutcomeOverviewRequest>())).ReturnsAsync((OutcomeApplicationViewModel)null);

            var result = await _controller.ViewOutcome(_applicationId) as RedirectToActionResult;

            Assert.AreEqual("Home", result.ControllerName);
            Assert.AreEqual("Index", result.ActionName);
        }
    }
}
