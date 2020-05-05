﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.AssessorDashboardOrchestrator
{
    [TestFixture]
    public class GetNewApplicationsViewModelTests
    {
        private Mock<IRoatpAssessorApiClient> _apiClient;
        private Web.Services.AssessorDashboardOrchestrator _orchestrator;
        
        [SetUp]
        public void SetUp()
        {
            _apiClient = new Mock<IRoatpAssessorApiClient>();
            _orchestrator = new Web.Services.AssessorDashboardOrchestrator(_apiClient.Object);

            _apiClient.Setup(x => x.GetNewApplications(It.IsAny<string>())).ReturnsAsync(new List<RoatpAssessorApplicationSummary>());
            _apiClient.Setup(x => x.GetAssessorSummary(It.IsAny<string>())).ReturnsAsync(new RoatpAssessorSummary());
        }

        [Test]
        public async Task When_getting_new_applications_then_the_application_summary_is_returned()
        {
            var userId = "sdjfhnsrfdg";
            var summary = new RoatpAssessorSummary { NewApplications = 34, ModerationApplications = 43, InProgressApplications = 2, ClarificationApplications = 6 };
            
            _apiClient.Setup(x => x.GetAssessorSummary(userId)).ReturnsAsync(summary);

            var response = await _orchestrator.GetNewApplicationsViewModel(userId);

            Assert.AreEqual(summary.NewApplications, response.NewApplications);
            Assert.AreEqual(summary.ModerationApplications, response.ModerationApplications);
            Assert.AreEqual(summary.ClarificationApplications, response.ClarificationApplications);
            Assert.AreEqual(summary.InProgressApplications, response.InProgressApplications);
        }

        [Test]
        public async Task When_getting_new_applications_the_new_applications_for_the_user_are_returned()
        {
            var userId = "sdjfhnsrfdg";
            var applications = new List<RoatpAssessorApplicationSummary>
            {
                new RoatpAssessorApplicationSummary { ApplicationReferenceNumber = "sdjfs", Assessor1Name = "sdjfghdfgd", ProviderRoute = "Main", OrganisationName = "Org 1", Ukprn = "132436565", ApplicationId = Guid.NewGuid() },
                new RoatpAssessorApplicationSummary { ApplicationReferenceNumber = "fghhgfj", ProviderRoute = "Supporting", OrganisationName = "Org 2", Ukprn = "3465904568", ApplicationId = Guid.NewGuid() }
            };

            _apiClient.Setup(x => x.GetNewApplications(userId)).ReturnsAsync(applications);

            var response = await _orchestrator.GetNewApplicationsViewModel(userId);

            Assert.AreEqual(applications.Count, response.Applications.Count);
            AssertApplicationsMatch(applications.First(), response.Applications.First());
            AssertApplicationsMatch(applications.Last(), response.Applications.Last());
        }

        private void AssertApplicationsMatch(RoatpAssessorApplicationSummary expected, ApplicationViewModel actual)
        {
            Assert.AreEqual(expected.ApplicationId, actual.ApplicationId);
            Assert.AreEqual(expected.OrganisationName, actual.OrganisationName);
            Assert.AreEqual(expected.ApplicationReferenceNumber, actual.ApplicationReferenceNumber);
            Assert.AreEqual(expected.Assessor1Name, actual.Assessor1);
            Assert.AreEqual(expected.Assessor2Name, actual.Assessor2);
            Assert.AreEqual(expected.ProviderRoute, actual.ProviderRoute);
            Assert.AreEqual(expected.SubmittedDate, actual.SubmittedDate);
            Assert.AreEqual(expected.Ukprn, actual.Ukprn);
        }
    }
}