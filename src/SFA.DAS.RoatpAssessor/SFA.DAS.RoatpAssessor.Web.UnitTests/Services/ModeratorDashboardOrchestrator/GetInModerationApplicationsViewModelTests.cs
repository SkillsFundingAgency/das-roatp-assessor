using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.ModeratorDashboardOrchestrator
{
    [TestFixture]
    public class GetInModerationApplicationsViewModelTests
    {
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applicationApiClient;
        private Web.Services.ModeratorDashboardOrchestrator _orchestrator;

        [SetUp]
        public void SetUp()
        {
            _applicationApiClient = new Mock<IRoatpApplicationApiClient>();
            _orchestrator = new Web.Services.ModeratorDashboardOrchestrator(_applicationApiClient.Object);

            _applicationApiClient.Setup(x => x.GetModerationApplications(_user.UserId())).ReturnsAsync(new List<RoatpModerationApplicationSummary>());
            _applicationApiClient.Setup(x => x.GetAssessorSummary(_user.UserId())).ReturnsAsync(new RoatpAssessorSummary());
        }

        [Test]
        public async Task When_getting_in_moderation_applications_then_the_application_summary_is_returned()
        {
            var userId = _user.UserId();
            var summary = new RoatpAssessorSummary { NewApplications = 34, ModerationApplications = 43, InProgressApplications = 2, ClarificationApplications = 6 };

            _applicationApiClient.Setup(x => x.GetAssessorSummary(userId)).ReturnsAsync(summary);

            var response = await _orchestrator.GetInModerationApplicationsViewModel(userId);

            Assert.AreEqual(summary.NewApplications, response.NewApplications);
            Assert.AreEqual(summary.ModerationApplications, response.ModerationApplications);
            Assert.AreEqual(summary.ClarificationApplications, response.ClarificationApplications);
            Assert.AreEqual(summary.InProgressApplications, response.InProgressApplications);
        }

        [Test]
        public async Task When_getting_in_moderation_applications_the_applications__are_returned()
        {
            var userId = _user.UserId();
            var applications = new List<RoatpModerationApplicationSummary>
            {
                new RoatpModerationApplicationSummary { ApplicationReferenceNumber = "sdjfs", Assessor1Name = "sdjfghdfgd", ProviderRoute = "Main", OrganisationName = "Org 1", Ukprn = "132436565", ApplicationId = Guid.NewGuid(), Assessor1UserId = "flggfdg", Status = ModerationStatus.InModeration },
                new RoatpModerationApplicationSummary { ApplicationReferenceNumber = "fghhgfj", ProviderRoute = "Supporting", OrganisationName = "Org 2", Ukprn = "3465904568", ApplicationId = Guid.NewGuid(), Assessor1UserId = "fbvkjghb", Assessor2UserId = "fdkgjgfdh", Status = ModerationStatus.New }
            };

            _applicationApiClient.Setup(x => x.GetModerationApplications(userId)).ReturnsAsync(applications);

            var response = await _orchestrator.GetInModerationApplicationsViewModel(userId);

            Assert.AreEqual(applications.Count, response.Applications.Count);
            AssertApplicationsMatch(applications.First(), response.Applications.First());
            AssertApplicationsMatch(applications.Last(), response.Applications.Last());
        }

        private void AssertApplicationsMatch(RoatpModerationApplicationSummary expected, ModerationApplicationViewModel actual)
        {
            Assert.AreEqual(expected.ApplicationId, actual.ApplicationId);
            Assert.AreEqual(expected.OrganisationName, actual.OrganisationName);
            Assert.AreEqual(expected.ApplicationReferenceNumber, actual.ApplicationReferenceNumber);
            Assert.AreEqual(expected.Assessor1Name, actual.Assessor1Name);
            Assert.AreEqual(expected.Assessor2Name, actual.Assessor2Name);
            Assert.AreEqual(expected.Assessor1UserId, actual.Assessor1UserId);
            Assert.AreEqual(expected.Assessor2UserId, actual.Assessor2UserId);
            Assert.AreEqual(expected.ProviderRoute, actual.ProviderRoute);
            Assert.AreEqual(expected.SubmittedDate, actual.SubmittedDate);
            Assert.AreEqual(expected.Ukprn, actual.Ukprn);
            Assert.AreEqual(expected.Status, actual.Status);
        }
    }
}
