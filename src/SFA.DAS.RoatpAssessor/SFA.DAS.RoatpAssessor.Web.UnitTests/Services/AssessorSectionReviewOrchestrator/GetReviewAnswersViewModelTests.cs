using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Services;


namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.AssessorSectionReviewOrchestrator
{
    [TestFixture]
    public class GetReviewAnswersViewModelTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpAssessorApiClient> _assessorApiClient;
        private Web.Services.AssessorSectionReviewOrchestrator _orchestrator;
        private GetReviewAnswersRequest _request;
        private Apply _application;
        private Contact _contact;
        private AssessorPage _assessorPage;
        private AssessorPageReviewOutcome _pageReviewOutcome;

        private readonly int _sequenceNumber = 4;
        private readonly int _sectionNumber = 2;
        private readonly string _pageId = "4200";
        private string _userId;

        [SetUp]
        public void SetUp()
        {
            var logger = new Mock<ILogger<Web.Services.AssessorSectionReviewOrchestrator>>();

            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _assessorApiClient = new Mock<IRoatpAssessorApiClient>();

            var supplementaryInformationService = new Mock<ISupplementaryInformationService>();

            _orchestrator = new Web.Services.AssessorSectionReviewOrchestrator(logger.Object, _applyApiClient.Object, _assessorApiClient.Object, supplementaryInformationService.Object);

            _userId = _user.UserId();

            _application = new Apply
            {
                ApplicationId = _applicationId,
                ApplyData = new ApplyData
                {
                    ApplyDetails = new ApplyDetails { }
                },
                Assessor1UserId = _userId
            };

            _contact = new Contact
            {
                Email = _userId,
                GivenNames = _user.GivenName(),
                FamilyName = _user.Surname()
            };

            _assessorPage = new AssessorPage
            {
                ApplicationId = _applicationId,
                SequenceNumber = _sequenceNumber,
                SectionNumber = _sectionNumber,
                PageId = _pageId,
                Questions = new List<Question>
                {
                    new Question { QuestionId = "Q1" }
                },
                Answers = new List<Answer>
                {
                    new Answer { QuestionId = "Q1", Value = "value" }
                }
            };

            _pageReviewOutcome = new AssessorPageReviewOutcome
            {
                ApplicationId = _applicationId,
                SequenceNumber = _sequenceNumber,
                SectionNumber = _sectionNumber,
                PageId = _pageId,
                UserId = _userId,
                Status = AssessorPageReviewStatus.Pass
            };

            _applyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(_application);

            _applyApiClient.Setup(x => x.GetContactForApplication(_applicationId)).ReturnsAsync(_contact);

            _assessorApiClient.Setup(x => x.GetAssessorPage(_applicationId, _sequenceNumber, _sectionNumber, _pageId))
                .ReturnsAsync(_assessorPage);

            _assessorApiClient.Setup(x => x.GetAssessorPageReviewOutcomesForSection(_applicationId, _sequenceNumber, _sectionNumber, _userId))
                .ReturnsAsync(new List<AssessorPageReviewOutcome> { _pageReviewOutcome });

            _assessorApiClient.Setup(x => x.GetAssessorPageReviewOutcome(_applicationId, _sequenceNumber, _sectionNumber, _pageId, _userId))
                .ReturnsAsync(_pageReviewOutcome);
        }

        [Test]
        public async Task GetReviewAnswersViewModel_returns_ViewModel()
        {
            _request = new GetReviewAnswersRequest(_applicationId, _userId, _sequenceNumber, _sectionNumber, _pageId, null);
            var result = await _orchestrator.GetReviewAnswersViewModel(_request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ApplicationId, Is.EqualTo(_assessorPage.ApplicationId));
            Assert.That(result.SequenceNumber, Is.EqualTo(_assessorPage.SequenceNumber));
            Assert.That(result.SectionNumber, Is.EqualTo(_assessorPage.SectionNumber));
            Assert.That(result.PageId, Is.EqualTo(_assessorPage.PageId));
            Assert.That(result.Status, Is.EqualTo(_pageReviewOutcome.Status));
            CollectionAssert.IsNotEmpty(result.Questions);
            CollectionAssert.IsNotEmpty(result.Answers);
        }
    }
}
