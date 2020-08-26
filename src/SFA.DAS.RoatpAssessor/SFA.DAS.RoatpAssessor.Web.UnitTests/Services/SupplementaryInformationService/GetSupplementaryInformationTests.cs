using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Consts;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.SupplementaryInformationService
{
    [TestFixture]
    public class GetSupplementaryInformationTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();

        private Mock<IRoatpAssessorApiClient> _assessorApiClient;
        private Web.Services.SupplementaryInformationService _service;

        [SetUp]
        public void SetUp()
        {
            _assessorApiClient = new Mock<IRoatpAssessorApiClient>();

            _service = new Web.Services.SupplementaryInformationService(_assessorApiClient.Object);
        }

        [Test]
        public async Task When_SupplementaryInformation_does_not_exist()
        {
            var pageId = Guid.NewGuid().ToString();

            var assessorPage = default(AssessorPage);

            _assessorApiClient.Setup(x => x.GetAssessorPage(_applicationId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(assessorPage);

            var result = await _service.GetSupplementaryInformation(_applicationId, pageId);

            CollectionAssert.IsEmpty(result);
        }

        [Test]
        public async Task When_SupplementaryInformation_does_exist()
        {
            var safeguardingPreventDutyPolicyPageId = RoatpWorkflowPageIds.SafeguardingPolicyIncludesPreventDutyPolicy;

            var assessorPage = new AssessorPage
            {
                ApplicationId = _applicationId,
                Questions = new List<Question>
                {
                    new Question { QuestionId = "Q1" }
                },
                Answers = new List<Answer>
                {
                    new Answer { QuestionId = "Q1", Value = "value" }
                }
            };

            _assessorApiClient.Setup(x => x.GetAssessorPage(_applicationId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(assessorPage);

            var result = await _service.GetSupplementaryInformation(_applicationId, safeguardingPreventDutyPolicyPageId);

            CollectionAssert.IsNotEmpty(result);
        }
    }
}
