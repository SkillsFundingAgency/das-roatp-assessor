using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.AssessorDashboardOrchestrator
{
    [TestFixture]
    public class AssignApplicationToAssessorTests
    {
        private Mock<IRoatpAssessorApiClient> _apiClient;
        private Web.Services.AssessorDashboardOrchestrator _orchestrator;
        
        [SetUp]
        public void SetUp()
        {
            _apiClient = new Mock<IRoatpAssessorApiClient>();
            _orchestrator = new Web.Services.AssessorDashboardOrchestrator(_apiClient.Object);
        }

        [Test]
        public async Task When_assigning_assessor_the_assessor_details_Are_stored()
        {
            var userId = "sdjfhnsrfdg";
            var userName = "sdjfhfsdg";
            var applicationId = Guid.NewGuid();
            var assessorNumber = 2;

            await _orchestrator.AssignApplicationToAssessor(applicationId, assessorNumber, userId, userName);

            _apiClient.Verify(x => x.AssignAssessor(applicationId, It.Is<AssignAssessorApplicationRequest>(r => r.AssessorUserId == userId && r.AssessorName == userName && r.AssessorNumber == assessorNumber)));
        }
    }
}
