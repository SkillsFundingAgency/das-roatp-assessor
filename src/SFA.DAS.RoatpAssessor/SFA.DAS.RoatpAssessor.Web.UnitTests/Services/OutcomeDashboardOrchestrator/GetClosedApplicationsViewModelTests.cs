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
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Outcome;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.OutcomeDashboardOrchestrator
{
    [TestFixture]
    public class GetClosedApplicationsViewModelTests
    {
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applicationApiClient;
        private Web.Services.OutcomeDashboardOrchestrator _orchestrator;

        [SetUp]
        public void SetUp()
        {
            _applicationApiClient = new Mock<IRoatpApplicationApiClient>();
            _orchestrator = new Web.Services.OutcomeDashboardOrchestrator(_applicationApiClient.Object);

            _applicationApiClient.Setup(x => x.GetClosedApplications(_user.UserId(), null, null, null)).ReturnsAsync(new List<ClosedApplicationSummary>());
            _applicationApiClient.Setup(x => x.GetApplicationCounts(_user.UserId(), null)).ReturnsAsync(new ApplicationCounts());
        }

        [Test]
        public async Task When_getting_in_closed_applications_then_the_application_summary_is_returned()
        {
            var userId = _user.UserId();
            var summary = new ApplicationCounts { NewApplications = 34, ModerationApplications = 43, InProgressApplications = 2, ClarificationApplications = 6, ClosedApplications = 1 };

            _applicationApiClient.Setup(x => x.GetApplicationCounts(userId, null)).ReturnsAsync(summary);

            var response = await _orchestrator.GetClosedApplicationsViewModel(userId, null, null, null);

            Assert.AreEqual(summary.NewApplications, response.NewApplications);
            Assert.AreEqual(summary.InProgressApplications, response.InProgressApplications);
            Assert.AreEqual(summary.ModerationApplications, response.ModerationApplications);
            Assert.AreEqual(summary.ClarificationApplications, response.ClarificationApplications);
            Assert.AreEqual(summary.ClosedApplications, response.ClosedApplications);
        }

        [Test]
        public async Task When_getting_closed_applications_the_applications__are_returned()
        {
            var userId = _user.UserId();
            var applications = new List<ClosedApplicationSummary>
            {
                new ClosedApplicationSummary { ApplicationReferenceNumber = "sdjfs", ProviderRoute = "Main", OrganisationName = "Org 1", Ukprn = "132436565", ApplicationId = Guid.NewGuid(), OutcomeMadeBy = "flggfdg", ModerationStatus = ModerationStatus.Pass, OutcomeMadeDate = DateTime.UtcNow },
                new ClosedApplicationSummary { ApplicationReferenceNumber = "fghhgfj", ProviderRoute = "Supporting", OrganisationName = "Org 2", Ukprn = "3465904568", ApplicationId = Guid.NewGuid(), OutcomeMadeBy = "fbvkjghb", ModerationStatus = ModerationStatus.Fail, OutcomeMadeDate = DateTime.UtcNow  }
            };

            _applicationApiClient.Setup(x => x.GetClosedApplications(userId, null, null, null)).ReturnsAsync(applications);

            var response = await _orchestrator.GetClosedApplicationsViewModel(userId, null, null, null);

            Assert.AreEqual(applications.Count, response.Applications.Count);
            AssertApplicationsMatch(applications.First(), response.Applications.First());
            AssertApplicationsMatch(applications.Last(), response.Applications.Last());
        }

        private void AssertApplicationsMatch(ClosedApplicationSummary expected, ClosedApplicationViewModel actual)
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
            Assert.AreEqual(expected.OutcomeMadeBy, actual.OutcomeMadeBy);
            Assert.AreEqual(expected.ModerationStatus, actual.ModerationStatus);
            Assert.AreEqual(expected.OutcomeMadeDate, actual.OutcomeMadeDate);
        }
    }
}
