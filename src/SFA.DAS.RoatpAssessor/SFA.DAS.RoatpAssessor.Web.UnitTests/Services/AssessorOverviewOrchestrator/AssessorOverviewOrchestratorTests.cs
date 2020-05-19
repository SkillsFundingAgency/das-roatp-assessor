using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.UnitTests.MockedObjects;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.AssessorOverviewOrchestrator
{
    [TestFixture]
    public class AssessorOverviewOrchestratorTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<ILogger<Web.Services.AssessorOverviewOrchestrator>> _logger;
        private Mock<IRoatpApplicationApiClient> _apiClient;
        private Web.Services.AssessorOverviewOrchestrator _orchestrator;


        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<Web.Services.AssessorOverviewOrchestrator>>();
            _apiClient = new Mock<IRoatpApplicationApiClient>();
            _orchestrator = new Web.Services.AssessorOverviewOrchestrator(_logger.Object, _apiClient.Object);
        }

        [TestCase(null)]
        [TestCase(AssessorPageReviewStatus.Pass)]
        [TestCase(AssessorPageReviewStatus.Fail)]
        [TestCase(AssessorPageReviewStatus.InProgress)]
        public void SetSectionStatus_Single_PageReviewOutcome(string status)
        {
            List<PageReviewOutcome> sectionPageReviewOutcomes = new List<PageReviewOutcome>
            {
                new PageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    Status = status
                }
            };

            var sectionStatus = _orchestrator.SetSectionStatus(sectionPageReviewOutcomes);
            Assert.AreSame(status, sectionStatus);
        }

        [TestCase(null, null, null, null)]
        [TestCase(AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, AssessorSectionStatus.Pass)]
        [TestCase(AssessorPageReviewStatus.Fail, AssessorPageReviewStatus.Fail, AssessorPageReviewStatus.Fail, AssessorSectionStatus.Fail)]
        [TestCase(AssessorPageReviewStatus.InProgress, null, null, AssessorSectionStatus.InProgress)]
        [TestCase(null, AssessorPageReviewStatus.Pass,  null, AssessorSectionStatus.InProgress)]
        [TestCase(AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, null, AssessorSectionStatus.InProgress)]
        [TestCase(null, null, AssessorPageReviewStatus.Fail, AssessorSectionStatus.InProgress)]
        [TestCase(AssessorPageReviewStatus.Pass, null, AssessorPageReviewStatus.Fail, AssessorSectionStatus.InProgress)]
        [TestCase(AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Fail, "1 " + AssessorSectionStatus.FailOutOf + " 3")]
        [TestCase(AssessorPageReviewStatus.Fail, AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Fail, "2 " + AssessorSectionStatus.FailOutOf + " 3")]
        public void SetSectionStatus_Muliple_PageReviewOutcomes(string statusOne, string statusTwo, string statusThree, string statusExpected)
        {
            List<PageReviewOutcome> sectionPageReviewOutcomes = new List<PageReviewOutcome>
            {
                new PageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    Status = statusOne
                },
                new PageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    Status = statusTwo
                },
                new PageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    Status = statusThree
                }
            };

            var sectionStatus = _orchestrator.SetSectionStatus(sectionPageReviewOutcomes);
            Assert.AreEqual(statusExpected, sectionStatus);
        }
    }
}
