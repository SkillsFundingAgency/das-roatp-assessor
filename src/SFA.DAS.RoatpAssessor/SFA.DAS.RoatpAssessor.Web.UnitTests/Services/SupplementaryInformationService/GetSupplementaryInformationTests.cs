using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
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

        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Web.Services.SupplementaryInformationService _service;

        [SetUp]
        public void SetUp()
        {
            var logger = new Mock<ILogger<Web.Services.SupplementaryInformationService>>();
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();

            _service = new Web.Services.SupplementaryInformationService(logger.Object, _applyApiClient.Object);
        }

        [Test]
        public async Task When_SupplementaryInformation_does_not_exist()
        {
            var pageId = Guid.NewGuid().ToString();

            var assessorPage = default(AssessorPage);

            _applyApiClient.Setup(x => x.GetAssessorPage(_applicationId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
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
                Questions = new List<AssessorQuestion>
                {      
                    new AssessorQuestion { QuestionId = "Q1" }
                },
                Answers = new List<AssessorAnswer>
                {
                    new AssessorAnswer { QuestionId = "Q1", Value = "value" }
                }
            };

            _applyApiClient.Setup(x => x.GetAssessorPage(_applicationId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(assessorPage);

            var result = await _service.GetSupplementaryInformation(_applicationId, safeguardingPreventDutyPolicyPageId);

            CollectionAssert.IsNotEmpty(result);
        }
    }
}
