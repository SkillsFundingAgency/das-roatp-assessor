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

            var sectionStatus = _orchestrator.GetSectionStatus(sectionPageReviewOutcomes,false);
            Assert.AreSame(status, sectionStatus);
        }

        [TestCase(null, null, null, null, false)]
        [TestCase(AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, AssessorSectionStatus.Pass, false)]
        [TestCase(AssessorPageReviewStatus.Fail, AssessorPageReviewStatus.Fail, AssessorPageReviewStatus.Fail, AssessorSectionStatus.Fail, false)]
        [TestCase(AssessorPageReviewStatus.InProgress, null, null, AssessorSectionStatus.InProgress, false)]
        [TestCase(null, AssessorPageReviewStatus.Pass,  null, AssessorSectionStatus.InProgress, false)]
        [TestCase(AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, null, AssessorSectionStatus.InProgress, false)]
        [TestCase(null, null, AssessorPageReviewStatus.Fail, AssessorSectionStatus.InProgress, false)]
        [TestCase(AssessorPageReviewStatus.Pass, null, AssessorPageReviewStatus.Fail, AssessorSectionStatus.InProgress, false)]
        [TestCase(AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Fail, "1 " + AssessorSectionStatus.FailOutOf + " 3", false)]
        [TestCase(AssessorPageReviewStatus.Fail, AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Fail, "2 " + AssessorSectionStatus.FailOutOf + " 3", false)]
        [TestCase(null, null, null, null, true)]
        [TestCase(AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, AssessorSectionStatus.Pass, true)]
        [TestCase(AssessorPageReviewStatus.Fail, AssessorPageReviewStatus.Fail, AssessorPageReviewStatus.Fail, AssessorSectionStatus.Fail, true)]
        [TestCase(AssessorPageReviewStatus.InProgress, null, null, AssessorSectionStatus.InProgress, true)]
        [TestCase(null, AssessorPageReviewStatus.Pass, null, AssessorSectionStatus.InProgress, true)]
        [TestCase(AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, null, AssessorSectionStatus.InProgress, true)]
        [TestCase(null, null, AssessorPageReviewStatus.Fail, AssessorSectionStatus.InProgress, true)]
        [TestCase(AssessorPageReviewStatus.Pass, null, AssessorPageReviewStatus.Fail, AssessorSectionStatus.InProgress, true)]
        [TestCase(AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Fail, AssessorSectionStatus.Fail, true)]
        [TestCase(AssessorPageReviewStatus.Fail, AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Fail, AssessorSectionStatus.Fail, true)]
        public void SetSectionStatus_Muliple_PageReviewOutcomes(string statusOne, string statusTwo, string statusThree, string statusExpected, bool sectionStatusFlag)
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

            var sectionStatus = _orchestrator.GetSectionStatus(sectionPageReviewOutcomes,sectionStatusFlag);
            Assert.AreEqual(statusExpected, sectionStatus);
        }
    }
}
