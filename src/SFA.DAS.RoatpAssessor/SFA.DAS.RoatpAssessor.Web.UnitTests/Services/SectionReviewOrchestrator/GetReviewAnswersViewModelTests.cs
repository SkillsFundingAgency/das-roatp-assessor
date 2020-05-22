using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.UnitTests.MockedObjects;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.SectionReviewOrchestrator
{
    [TestFixture]
    public class GetReviewAnswersViewModelTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Web.Services.SectionReviewOrchestrator _orchestrator;
        private GetReviewAnswersRequest _request;
        private Apply _application;
        private Contact _contact;
        private AssessorPage _assessorPage;
        private PageReviewOutcome _pageReviewOutcome;

        private readonly int _sequenceNumber = 4;
        private readonly int _sectionNumber = 2;
        private readonly string _pageId = "4200";
        private string _userId;

        [SetUp]
        public void SetUp()
        {
            var logger = new Mock<ILogger<Web.Services.SectionReviewOrchestrator>>();

            _applyApiClient = new Mock<IRoatpApplicationApiClient>();

            var supplementaryInformationService = new Mock<ISupplementaryInformationService>();

            _orchestrator = new Web.Services.SectionReviewOrchestrator(logger.Object, _applyApiClient.Object, supplementaryInformationService.Object);

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
                Questions = new List<AssessorQuestion>
                {
                    new AssessorQuestion { QuestionId = "Q1" }
                },
                Answers = new List<AssessorAnswer>
                {
                    new AssessorAnswer { QuestionId = "Q1", Value = "value" }
                }
            };

            _pageReviewOutcome = new PageReviewOutcome
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

            _applyApiClient.Setup(x => x.GetAssessorPage(_applicationId, _sequenceNumber, _sectionNumber, _pageId))
                .ReturnsAsync(_assessorPage);

            _applyApiClient.Setup(x => x.GetAssessorReviewOutcomesPerSection(_applicationId, _sequenceNumber, _sectionNumber, It.IsAny<int>(), _userId))
                .ReturnsAsync(new List<PageReviewOutcome> { _pageReviewOutcome });

            _applyApiClient.Setup(x => x.GetPageReviewOutcome(_applicationId, _sequenceNumber, _sectionNumber, _pageId, It.IsAny<int>(), _userId))
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

        [Test]
        public async Task When_there_is_no_page_id_provided_and_there_are_no_existing_outcomes_then_outcomes_are_submitted()
        {
            _applyApiClient.Setup(x => x.GetAssessorPage(_applicationId, _sequenceNumber, _sectionNumber, null)).ReturnsAsync(_assessorPage);
            _applyApiClient.Setup(x => x.GetAssessorReviewOutcomesPerSection(_applicationId, _sequenceNumber, _sectionNumber, (int)AssessorType.FirstAssessor, _userId)).ReturnsAsync((List<PageReviewOutcome>)null);

            _request = new GetReviewAnswersRequest(_applicationId, _userId, _sequenceNumber, _sectionNumber, null, null);
            await _orchestrator.GetReviewAnswersViewModel(_request);

            _applyApiClient.Verify(x => x.SubmitAssessorPageOutcome(_applicationId, _sequenceNumber, _sectionNumber, _pageId, (int)AssessorType.FirstAssessor, _userId, null, null));
        }

        [Test]
        public async Task When_there_is_no_page_id_provided_and_there_are_no_existing_outcomes_then_outcomes_are_submitted_for_next_pages()
        {
            _assessorPage.NextPageId = "NP01";
            _applyApiClient.Setup(x => x.GetAssessorPage(_applicationId, _sequenceNumber, _sectionNumber, null)).ReturnsAsync(_assessorPage);
            _applyApiClient.Setup(x => x.GetAssessorReviewOutcomesPerSection(_applicationId, _sequenceNumber, _sectionNumber, (int)AssessorType.FirstAssessor, _userId)).ReturnsAsync((List<PageReviewOutcome>)null);

            var nextPage = new AssessorPage { NextPageId = "NP02" };
            _applyApiClient.Setup(x => x.GetAssessorPage(_applicationId, _sequenceNumber, _sectionNumber, _assessorPage.NextPageId)).ReturnsAsync(nextPage);

            _applyApiClient.Setup(x => x.GetAssessorPage(_applicationId, _sequenceNumber, _sectionNumber, nextPage.NextPageId)).ReturnsAsync(new AssessorPage());

            _request = new GetReviewAnswersRequest(_applicationId, _userId, _sequenceNumber, _sectionNumber, null, null);
            await _orchestrator.GetReviewAnswersViewModel(_request);

            _applyApiClient.Verify(x => x.SubmitAssessorPageOutcome(_applicationId, _sequenceNumber, _sectionNumber, _assessorPage.NextPageId, (int)AssessorType.FirstAssessor, _userId, null, null));
            _applyApiClient.Verify(x => x.SubmitAssessorPageOutcome(_applicationId, _sequenceNumber, _sectionNumber, nextPage.NextPageId, (int)AssessorType.FirstAssessor, _userId, null, null));
        }
    }
}
