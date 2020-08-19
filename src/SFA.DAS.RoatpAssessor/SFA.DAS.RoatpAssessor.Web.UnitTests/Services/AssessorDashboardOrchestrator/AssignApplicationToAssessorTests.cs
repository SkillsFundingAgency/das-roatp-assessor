using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.AssessorDashboardOrchestrator
{
    [TestFixture]
    public class AssignApplicationToAssessorTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
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
        }

        [Test]
        public async Task When_assigning_assessor_the_assessor_details_Are_stored()
        {
            var userId = _user.UserId();
            var userName = _user.UserDisplayName();
            var assessorNumber = 2;

            await _orchestrator.AssignApplicationToAssessor(_applicationId, assessorNumber, userId, userName);

            _assessorApiClient.Verify(x => x.AssignAssessor(_applicationId, It.Is<AssignAssessorApplicationRequest>(r => r.AssessorUserId == userId && r.AssessorName == userName && r.AssessorNumber == assessorNumber)));
        }
    }
}
