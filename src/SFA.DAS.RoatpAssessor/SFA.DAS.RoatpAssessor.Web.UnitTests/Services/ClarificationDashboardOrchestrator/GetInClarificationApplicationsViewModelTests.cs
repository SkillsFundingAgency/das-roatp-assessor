using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.ClarificationDashboardOrchestrator
{
    [TestFixture]
    public class GetInClarificationApplicationsViewModelTests
    {
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applicationApiClient;
        private Web.Services.ClarificationDashboardOrchestrator _orchestrator;

        [SetUp]
        public void SetUp()
        {
            _applicationApiClient = new Mock<IRoatpApplicationApiClient>();
            _orchestrator = new Web.Services.ClarificationDashboardOrchestrator(_applicationApiClient.Object);

            _applicationApiClient.Setup(x => x.GetInClarificationApplications(_user.UserId())).ReturnsAsync(new List<ClarificationApplicationSummary>());
            _applicationApiClient.Setup(x => x.GetApplicationCounts(_user.UserId())).ReturnsAsync(new ApplicationCounts());
        }

        [Test]
        public async Task When_getting_in_clarification_applications_then_the_application_summary_is_returned()
        {
            var userId = _user.UserId();
            var summary = new ApplicationCounts { NewApplications = 34, ModerationApplications = 43, InProgressApplications = 2, ClarificationApplications = 6, ClosedApplications = 1 };

            _applicationApiClient.Setup(x => x.GetApplicationCounts(userId)).ReturnsAsync(summary);

            var response = await _orchestrator.GetInClarificationApplicationsViewModel(userId);

            Assert.AreEqual(summary.NewApplications, response.NewApplications);
            Assert.AreEqual(summary.InProgressApplications, response.InProgressApplications);
            Assert.AreEqual(summary.ModerationApplications, response.ModerationApplications);
            Assert.AreEqual(summary.ClarificationApplications, response.ClarificationApplications);
            Assert.AreEqual(summary.ClosedApplications, response.ClosedApplications);
        }

        [Test]
        public async Task When_getting_in_clarification_applications_the_applications__are_returned()
        {
            var userId = _user.UserId();
            var applications = new List<ClarificationApplicationSummary>
            {
                new ClarificationApplicationSummary { ApplicationReferenceNumber = "sdjfs", ProviderRoute = "Main", OrganisationName = "Org 1", Ukprn = "132436565", ApplicationId = Guid.NewGuid(), ModeratorName = "flggfdg", ClarificationRequestedOn = DateTime.UtcNow },
                new ClarificationApplicationSummary { ApplicationReferenceNumber = "fghhgfj", ProviderRoute = "Supporting", OrganisationName = "Org 2", Ukprn = "3465904568", ApplicationId = Guid.NewGuid(), ModeratorName = "fbvkjghb", ClarificationRequestedOn = DateTime.UtcNow }
            };

            _applicationApiClient.Setup(x => x.GetInClarificationApplications(userId)).ReturnsAsync(applications);

            var response = await _orchestrator.GetInClarificationApplicationsViewModel(userId);

            Assert.AreEqual(applications.Count, response.Applications.Count);
            AssertApplicationsMatch(applications.First(), response.Applications.First());
            AssertApplicationsMatch(applications.Last(), response.Applications.Last());
        }

        private void AssertApplicationsMatch(ClarificationApplicationSummary expected, ClarificationApplicationViewModel actual)
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
            Assert.AreEqual(expected.ApplicationStatus, actual.ApplicationStatus);
            Assert.AreEqual(expected.Ukprn, actual.Ukprn);
            Assert.AreEqual(expected.ModeratorName, actual.ModeratorName);
            Assert.AreEqual(expected.ClarificationRequestedOn, actual.ClarificationRequestedDate);
        }
    }
}
