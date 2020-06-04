using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.AdminService.Common.Extensions;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.AssessorDashboardOrchestrator
{
    [TestFixture]
    public class AssignApplicationToAssessorTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

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
            var userId = _user.UserId();
            var userName = _user.UserDisplayName();
            var assessorNumber = 2;

            await _orchestrator.AssignApplicationToAssessor(_applicationId, assessorNumber, userId, userName);

            _apiClient.Verify(x => x.AssignAssessor(_applicationId, It.Is<AssignAssessorApplicationRequest>(r => r.AssessorUserId == userId && r.AssessorName == userName && r.AssessorNumber == assessorNumber)));
        }
    }
}
