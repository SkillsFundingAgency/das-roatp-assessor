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
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Services;


namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.ClarificationSectionReviewOrchestrator
{
    [TestFixture]
    public class GetReviewAnswersViewModelTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpClarificationApiClient> _clarificationApiClient;
        private Web.Services.ClarificationSectionReviewOrchestrator _orchestrator;
        private GetReviewAnswersRequest _request;
        private Apply _application;
        private Contact _contact;
        private ClarificationPage _clarificationPage;
        private ClarificationPageReviewOutcome _pageReviewOutcome;

        private readonly int _sequenceNumber = 4;
        private readonly int _sectionNumber = 2;
        private readonly string _pageId = "4200";
        private string _userId;
        private string _userName;

        [SetUp]
        public void SetUp()
        {
            var logger = new Mock<ILogger<Web.Services.ClarificationSectionReviewOrchestrator>>();

            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _clarificationApiClient = new Mock<IRoatpClarificationApiClient>();

            var supplementaryInformationService = new Mock<ISupplementaryInformationService>();

            _orchestrator = new Web.Services.ClarificationSectionReviewOrchestrator(logger.Object, _applyApiClient.Object, _clarificationApiClient.Object, supplementaryInformationService.Object);

            _userId = _user.UserId();
            _userName = _user.UserDisplayName();

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

            _clarificationPage = new ClarificationPage
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

            _pageReviewOutcome = new ClarificationPageReviewOutcome
            {
                ApplicationId = _applicationId,
                SequenceNumber = _sequenceNumber,
                SectionNumber = _sectionNumber,
                PageId = _pageId,
                UserId = _userId,
                UserName = _userName,
                Status = ClarificationPageReviewStatus.Pass,
                ModeratorUserId = _userId,
                ModeratorUserName = _userName,
                ModeratorReviewStatus = ApplyTypes.Moderator.ModeratorPageReviewStatus.Fail,
                ModeratorReviewComment = "Not Good",
                ClarificationResponse = "Response",
                ClarificationFile = null
            };

            _applyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(_application);

            _applyApiClient.Setup(x => x.GetContactForApplication(_applicationId)).ReturnsAsync(_contact);

            _clarificationApiClient.Setup(x => x.GetClarificationPage(_applicationId, _sequenceNumber, _sectionNumber, _pageId))
                .ReturnsAsync(_clarificationPage);

            _clarificationApiClient.Setup(x => x.GetClarificationPageReviewOutcomesForSection(_applicationId, _sequenceNumber, _sectionNumber, _userId))
                .ReturnsAsync(new List<ClarificationPageReviewOutcome> { _pageReviewOutcome });

            _clarificationApiClient.Setup(x => x.GetClarificationPageReviewOutcome(_applicationId, _sequenceNumber, _sectionNumber, _pageId, _userId))
                .ReturnsAsync(_pageReviewOutcome);
        }

        [Test]
        public async Task GetReviewAnswersViewModel_returns_ViewModel()
        {
            _request = new GetReviewAnswersRequest(_applicationId, _userId, _sequenceNumber, _sectionNumber, _pageId, null);
            var result = await _orchestrator.GetReviewAnswersViewModel(_request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ApplicationId, Is.EqualTo(_clarificationPage.ApplicationId));
            Assert.That(result.SequenceNumber, Is.EqualTo(_clarificationPage.SequenceNumber));
            Assert.That(result.SectionNumber, Is.EqualTo(_clarificationPage.SectionNumber));
            Assert.That(result.PageId, Is.EqualTo(_clarificationPage.PageId));
            Assert.That(result.Status, Is.EqualTo(_pageReviewOutcome.Status));
            Assert.That(result.ClarificationResponse, Is.EqualTo(_pageReviewOutcome.ClarificationResponse));
            Assert.That(result.ClarificationFile, Is.EqualTo(_pageReviewOutcome.ClarificationFile));
            Assert.That(result.ModerationOutcome.ModeratorUserId, Is.EqualTo(_pageReviewOutcome.ModeratorUserId));
            Assert.That(result.ModerationOutcome.ModeratorUserName, Is.EqualTo(_pageReviewOutcome.ModeratorUserName));
            Assert.That(result.ModerationOutcome.ModeratorReviewStatus, Is.EqualTo(_pageReviewOutcome.ModeratorReviewStatus));
            Assert.That(result.ModerationOutcome.ModeratorReviewComment, Is.EqualTo(_pageReviewOutcome.ModeratorReviewComment));
            CollectionAssert.IsNotEmpty(result.Questions);
            CollectionAssert.IsNotEmpty(result.Answers);
        }
    }
}
