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
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Services;


namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.ModeratorSectionReviewOrchestrator
{
    [TestFixture]
    public class GetReviewAnswersViewModelTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpModerationApiClient> _moderationApiClient;
        private Web.Services.ModeratorSectionReviewOrchestrator _orchestrator;
        private GetReviewAnswersRequest _request;
        private Apply _application;
        private Contact _contact;
        private ModeratorPage _moderatorPage;
        private ModeratorPageReviewOutcome _pageReviewOutcome;

        private readonly int _sequenceNumber = 4;
        private readonly int _sectionNumber = 2;
        private readonly string _pageId = "4200";
        private string _userId;

        [SetUp]
        public void SetUp()
        {
            var logger = new Mock<ILogger<Web.Services.ModeratorSectionReviewOrchestrator>>();

            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _moderationApiClient = new Mock<IRoatpModerationApiClient>();

            var supplementaryInformationService = new Mock<ISupplementaryInformationService>();

            _orchestrator = new Web.Services.ModeratorSectionReviewOrchestrator(logger.Object, _applyApiClient.Object, _moderationApiClient.Object, supplementaryInformationService.Object);

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

            _moderatorPage = new ModeratorPage
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

            _pageReviewOutcome = new ModeratorPageReviewOutcome
            {
                ApplicationId = _applicationId,
                SequenceNumber = _sequenceNumber,
                SectionNumber = _sectionNumber,
                PageId = _pageId,
                UserId = _userId,
                Status = ModeratorPageReviewStatus.Pass
            };

            _applyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(_application);

            _applyApiClient.Setup(x => x.GetContactForApplication(_applicationId)).ReturnsAsync(_contact);

            _moderationApiClient.Setup(x => x.GetModeratorPage(_applicationId, _sequenceNumber, _sectionNumber, _pageId))
                .ReturnsAsync(_moderatorPage);

            _moderationApiClient.Setup(x => x.GetModeratorPageReviewOutcomesForSection(_applicationId, _sequenceNumber, _sectionNumber, _userId))
                .ReturnsAsync(new List<ModeratorPageReviewOutcome> { _pageReviewOutcome });

            _moderationApiClient.Setup(x => x.GetModeratorPageReviewOutcome(_applicationId, _sequenceNumber, _sectionNumber, _pageId, _userId))
                .ReturnsAsync(_pageReviewOutcome);
        }

        [Test]
        public async Task GetReviewAnswersViewModel_returns_ViewModel()
        {
            _request = new GetReviewAnswersRequest(_applicationId, _userId, _sequenceNumber, _sectionNumber, _pageId, null);
            var result = await _orchestrator.GetReviewAnswersViewModel(_request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ApplicationId, Is.EqualTo(_moderatorPage.ApplicationId));
            Assert.That(result.SequenceNumber, Is.EqualTo(_moderatorPage.SequenceNumber));
            Assert.That(result.SectionNumber, Is.EqualTo(_moderatorPage.SectionNumber));
            Assert.That(result.PageId, Is.EqualTo(_moderatorPage.PageId));
            Assert.That(result.Status, Is.EqualTo(_pageReviewOutcome.Status));
            CollectionAssert.IsNotEmpty(result.Questions);
            CollectionAssert.IsNotEmpty(result.Answers);
        }

        [Test]
        public async Task When_there_is_no_page_id_provided_and_there_are_no_existing_outcomes_then_outcomes_are_submitted()
        {
            _moderationApiClient.Setup(x => x.GetModeratorPage(_applicationId, _sequenceNumber, _sectionNumber, null)).ReturnsAsync(_moderatorPage);
            _moderationApiClient.Setup(x => x.GetModeratorPageReviewOutcomesForSection(_applicationId, _sequenceNumber, _sectionNumber, _userId)).ReturnsAsync((List<ModeratorPageReviewOutcome>)null);

            _request = new GetReviewAnswersRequest(_applicationId, _userId, _sequenceNumber, _sectionNumber, null, null);
            await _orchestrator.GetReviewAnswersViewModel(_request);

            _moderationApiClient.Verify(x => x.SubmitModeratorPageReviewOutcome(_applicationId, _sequenceNumber, _sectionNumber, _pageId, _userId, null, null, null));
        }

        [Test]
        public async Task When_there_is_no_page_id_provided_and_there_are_no_existing_outcomes_then_outcomes_are_submitted_for_next_pages()
        {
            _moderatorPage.NextPageId = "NP01";
            _moderationApiClient.Setup(x => x.GetModeratorPage(_applicationId, _sequenceNumber, _sectionNumber, null)).ReturnsAsync(_moderatorPage);
            _moderationApiClient.Setup(x => x.GetModeratorPageReviewOutcomesForSection(_applicationId, _sequenceNumber, _sectionNumber, _userId)).ReturnsAsync((List<ModeratorPageReviewOutcome>)null);

            var nextPage = new ModeratorPage { NextPageId = "NP02" };
            _moderationApiClient.Setup(x => x.GetModeratorPage(_applicationId, _sequenceNumber, _sectionNumber, _moderatorPage.NextPageId)).ReturnsAsync(nextPage);

            _moderationApiClient.Setup(x => x.GetModeratorPage(_applicationId, _sequenceNumber, _sectionNumber, nextPage.NextPageId)).ReturnsAsync(new ModeratorPage());

            _request = new GetReviewAnswersRequest(_applicationId, _userId, _sequenceNumber, _sectionNumber, null, null);
            await _orchestrator.GetReviewAnswersViewModel(_request);

            _moderationApiClient.Verify(x => x.SubmitModeratorPageReviewOutcome(_applicationId, _sequenceNumber, _sectionNumber, _moderatorPage.NextPageId, _userId, null, null, null));
            _moderationApiClient.Verify(x => x.SubmitModeratorPageReviewOutcome(_applicationId, _sequenceNumber, _sectionNumber, nextPage.NextPageId, _userId, null, null, null));
        }
    }
}
