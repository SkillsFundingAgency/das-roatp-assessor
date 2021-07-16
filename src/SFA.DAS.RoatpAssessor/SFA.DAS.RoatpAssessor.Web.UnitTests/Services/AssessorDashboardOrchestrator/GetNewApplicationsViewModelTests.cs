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
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.AssessorDashboardOrchestrator
{
    [TestFixture]
    public class GetNewApplicationsViewModelTests
    {
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applicationApiClient;
        private Mock<IRoatpAssessorApiClient> _assessorApiClient;
        private Web.Services.AssessorDashboardOrchestrator _orchestrator;

        [SetUp]
        public void SetUp()
        {
            _applicationApiClient = new Mock<IRoatpApplicationApiClient>();
            _assessorApiClient = new Mock<IRoatpAssessorApiClient>();
            _orchestrator = new Web.Services.AssessorDashboardOrchestrator(_applicationApiClient.Object, _assessorApiClient.Object);

            _applicationApiClient.Setup(x => x.GetNewApplications(_user.UserId() ,null, null, null)).ReturnsAsync(new List<AssessorApplicationSummary>());
            _applicationApiClient.Setup(x => x.GetApplicationCounts(_user.UserId(), null)).ReturnsAsync(new ApplicationCounts());
        }

        [Test]
        public async Task When_getting_new_applications_then_the_application_summary_is_returned()
        {
            var userId = _user.UserId();
            var summary = new ApplicationCounts { NewApplications = 34, ModerationApplications = 43, InProgressApplications = 2, ClarificationApplications = 6, ClosedApplications = 1 };

            _applicationApiClient.Setup(x => x.GetApplicationCounts(userId, null)).ReturnsAsync(summary);

            var response = await _orchestrator.GetNewApplicationsViewModel(userId, null, null, null);

            Assert.AreEqual(summary.NewApplications, response.NewApplications);
            Assert.AreEqual(summary.InProgressApplications, response.InProgressApplications);
            Assert.AreEqual(summary.ModerationApplications, response.ModerationApplications);
            Assert.AreEqual(summary.ClarificationApplications, response.ClarificationApplications);
            Assert.AreEqual(summary.ClosedApplications, response.ClosedApplications);
        }

        [Test]
        public async Task When_getting_new_applications_the_new_applications_for_the_user_are_returned()
        {
            var userId = _user.UserId();
            var applications = new List<AssessorApplicationSummary>
            {
                new AssessorApplicationSummary { ApplicationReferenceNumber = "sdjfs", Assessor1Name = "sdjfghdfgd", ProviderRoute = "Main", OrganisationName = "Org 1", Ukprn = "132436565", ApplicationId = Guid.NewGuid(), Assessor1UserId = "edofig" },
                new AssessorApplicationSummary { ApplicationReferenceNumber = "fghhgfj", ProviderRoute = "Supporting", OrganisationName = "Org 2", Ukprn = "3465904568", ApplicationId = Guid.NewGuid() }
            };

            _applicationApiClient.Setup(x => x.GetNewApplications(userId, null, null, null)).ReturnsAsync(applications);

            var response = await _orchestrator.GetNewApplicationsViewModel(userId, null, null, null);

            Assert.AreEqual(applications.Count, response.Applications.Count);
            AssertApplicationsMatch(applications.First(), response.Applications.First());
            AssertApplicationsMatch(applications.Last(), response.Applications.Last());
        }

        private void AssertApplicationsMatch(AssessorApplicationSummary expected, ApplicationViewModel actual)
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
        }
    }
}
