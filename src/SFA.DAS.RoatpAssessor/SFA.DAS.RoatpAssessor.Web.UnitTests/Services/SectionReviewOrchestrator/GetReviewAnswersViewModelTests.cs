using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Services;


namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.SectionReviewOrchestrator
{
    [TestFixture]
    public class GetReviewAnswersViewModelTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpAssessorApiClient> _assessorApiClient;
        private Web.Services.SectionReviewOrchestrator _orchestrator;
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
            var logger = new Mock<ILogger<Web.Services.SectionReviewOrchestrator>>();

            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _assessorApiClient = new Mock<IRoatpAssessorApiClient>();

            var supplementaryInformationService = new Mock<ISupplementaryInformationService>();

            _orchestrator = new Web.Services.SectionReviewOrchestrator(logger.Object, _applyApiClient.Object, _assessorApiClient.Object, supplementaryInformationService.Object);

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

        [Test]
        public async Task When_there_is_no_page_id_provided_and_there_are_no_existing_outcomes_then_outcomes_are_submitted()
        {
            _assessorApiClient.Setup(x => x.GetAssessorPage(_applicationId, _sequenceNumber, _sectionNumber, null)).ReturnsAsync(_assessorPage);
            _assessorApiClient.Setup(x => x.GetAssessorPageReviewOutcomesForSection(_applicationId, _sequenceNumber, _sectionNumber, _userId)).ReturnsAsync((List<AssessorPageReviewOutcome>)null);

            _request = new GetReviewAnswersRequest(_applicationId, _userId, _sequenceNumber, _sectionNumber, null, null);
            await _orchestrator.GetReviewAnswersViewModel(_request);

            _assessorApiClient.Verify(x => x.SubmitAssessorPageReviewOutcome(_applicationId, _sequenceNumber, _sectionNumber, _pageId, _userId, null, null));
        }

        [Test]
        public async Task When_there_is_no_page_id_provided_and_there_are_no_existing_outcomes_then_outcomes_are_submitted_for_next_pages()
        {
            _assessorPage.NextPageId = "NP01";
            _assessorApiClient.Setup(x => x.GetAssessorPage(_applicationId, _sequenceNumber, _sectionNumber, null)).ReturnsAsync(_assessorPage);
            _assessorApiClient.Setup(x => x.GetAssessorPageReviewOutcomesForSection(_applicationId, _sequenceNumber, _sectionNumber, _userId)).ReturnsAsync((List<AssessorPageReviewOutcome>)null);

            var nextPage = new AssessorPage { NextPageId = "NP02" };
            _assessorApiClient.Setup(x => x.GetAssessorPage(_applicationId, _sequenceNumber, _sectionNumber, _assessorPage.NextPageId)).ReturnsAsync(nextPage);

            _assessorApiClient.Setup(x => x.GetAssessorPage(_applicationId, _sequenceNumber, _sectionNumber, nextPage.NextPageId)).ReturnsAsync(new AssessorPage());

            _request = new GetReviewAnswersRequest(_applicationId, _userId, _sequenceNumber, _sectionNumber, null, null);
            await _orchestrator.GetReviewAnswersViewModel(_request);

            _assessorApiClient.Verify(x => x.SubmitAssessorPageReviewOutcome(_applicationId, _sequenceNumber, _sectionNumber, _assessorPage.NextPageId, _userId, null, null));
            _assessorApiClient.Verify(x => x.SubmitAssessorPageReviewOutcome(_applicationId, _sequenceNumber, _sectionNumber, nextPage.NextPageId, _userId, null, null));
        }
    }
}
