using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.UnitTests.MockedObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.AssessorOverviewOrchestrator
{
    [TestFixture]
    public class AssessorOverviewOrchestratorTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<ILogger<Web.Services.AssessorOverviewOrchestrator>> _logger;
        private Mock<IRoatpApplicationApiClient> _apiClient;
        private Web.Services.AssessorOverviewOrchestrator _orchestrator;
        private string _userId;
        private Apply _application;
        private Contact _contact;
        private List<AssessorSequence> _sequences;
        private List<PageReviewOutcome> _outcomes;


        [SetUp]
        public void SetUp()
        {
            _logger = new Mock<ILogger<Web.Services.AssessorOverviewOrchestrator>>();
            _apiClient = new Mock<IRoatpApplicationApiClient>();
            _orchestrator = new Web.Services.AssessorOverviewOrchestrator(_logger.Object, _apiClient.Object);

            _userId = "user";
            _application = new Apply { ApplicationId = _applicationId, Assessor2UserId = _userId, Id = Guid.NewGuid(), OrganisationId = Guid.NewGuid(), Status = "Status", Assessor2ReviewStatus = "In Progress" };
            _contact = new Contact { Email = "email@address.com" };
            _sequences = new List<AssessorSequence>();
            _outcomes = new List<PageReviewOutcome>();

            _apiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(_application);
            _apiClient.Setup(x => x.GetContactForApplication(_applicationId)).ReturnsAsync(_contact);
            _apiClient.Setup(x => x.GetAssessorSequences(_applicationId)).ReturnsAsync(_sequences);
            _apiClient.Setup(x => x.GetAllAssessorReviewOutcomes(_applicationId, (int)AssessorType.SecondAssessor, _userId)).ReturnsAsync(_outcomes);
        }

        [TestCase(null)]
        [TestCase(AssessorPageReviewStatus.Pass)]
        [TestCase(AssessorPageReviewStatus.Fail)]
        [TestCase(AssessorPageReviewStatus.InProgress)]
        public void SetSectionStatus_Single_PageReviewOutcome(string status)
        {
            List<PageReviewOutcome> sectionPageReviewOutcomes = new List<PageReviewOutcome>
            {
                new PageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    Status = status
                }
            };

            var sectionStatus = _orchestrator.SetSectionStatus(sectionPageReviewOutcomes);
            Assert.AreSame(status, sectionStatus);
        }

        [TestCase(null, null, null, null)]
        [TestCase(AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, AssessorSectionStatus.Pass)]
        [TestCase(AssessorPageReviewStatus.Fail, AssessorPageReviewStatus.Fail, AssessorPageReviewStatus.Fail, AssessorSectionStatus.Fail)]
        [TestCase(AssessorPageReviewStatus.InProgress, null, null, AssessorSectionStatus.InProgress)]
        [TestCase(null, AssessorPageReviewStatus.Pass,  null, AssessorSectionStatus.InProgress)]
        [TestCase(AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, null, AssessorSectionStatus.InProgress)]
        [TestCase(null, null, AssessorPageReviewStatus.Fail, AssessorSectionStatus.InProgress)]
        [TestCase(AssessorPageReviewStatus.Pass, null, AssessorPageReviewStatus.Fail, AssessorSectionStatus.InProgress)]
        [TestCase(AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Fail, "1 " + AssessorSectionStatus.FailOutOf + " 3")]
        [TestCase(AssessorPageReviewStatus.Fail, AssessorPageReviewStatus.Pass, AssessorPageReviewStatus.Fail, "2 " + AssessorSectionStatus.FailOutOf + " 3")]
        public void SetSectionStatus_Muliple_PageReviewOutcomes(string statusOne, string statusTwo, string statusThree, string statusExpected)
        {
            List<PageReviewOutcome> sectionPageReviewOutcomes = new List<PageReviewOutcome>
            {
                new PageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    Status = statusOne
                },
                new PageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    Status = statusTwo
                },
                new PageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    Status = statusThree
                }
            };

            var sectionStatus = _orchestrator.SetSectionStatus(sectionPageReviewOutcomes);
            Assert.AreEqual(statusExpected, sectionStatus);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenApplicationNotFound_ThenNoViewModelIsReturned()
        {
            var applicationId = Guid.NewGuid();
            _apiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync((Apply) null);

            var result = await _orchestrator.GetOverviewViewModel(new GetApplicationOverviewRequest(applicationId, "userId"));

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenApplicationContactNotFound_ThenNoViewModelIsReturned()
        {
            var applicationId = Guid.NewGuid();
            _apiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            _apiClient.Setup(x => x.GetContactForApplication(applicationId)).ReturnsAsync((Contact) null);

            var result = await _orchestrator.GetOverviewViewModel(new GetApplicationOverviewRequest(applicationId, "userId"));

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenSequencesNotFound_ThenNoViewModelIsReturned()
        {
            var applicationId = Guid.NewGuid();
            _apiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            _apiClient.Setup(x => x.GetContactForApplication(applicationId)).ReturnsAsync(new Contact());
            _apiClient.Setup(x => x.GetAssessorSequences(applicationId)).ReturnsAsync((List<AssessorSequence>)null);

            var result = await _orchestrator.GetOverviewViewModel(new GetApplicationOverviewRequest(applicationId, "userId"));

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenThereAreNoSavedOutcomes_ThenTheApplicationIsNotReadyForModeration()
        {
            var result = await _orchestrator.GetOverviewViewModel(new GetApplicationOverviewRequest(_applicationId, _userId));

            AssertCommonProperties(result);
            Assert.IsFalse(result.IsReadyForModeration);
        }

        private void AssertCommonProperties(AssessorApplicationViewModel result)
        {
            Assert.AreEqual(_application.Id, result.Id);
            Assert.AreEqual(_application.ApplicationId, result.ApplicationId);
            Assert.AreEqual(_application.OrganisationId, result.OrgId);
            Assert.AreEqual(_contact.Email, result.ApplicantEmailAddress);
            Assert.AreEqual(_application.ApplicationStatus, result.ApplicationStatus);
            Assert.AreEqual(_application.Assessor2ReviewStatus, result.AssessorReviewStatus);
            Assert.AreSame(_sequences, result.Sequences);
            Assert.IsFalse(result.IsAssessorApproved);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenThereAreSavedOutcomesAndTheyAllPass_ThenTheOutcomesAreReturnedAndTheApplicationIsReadyForModeration()
        {
            var expectedStatus = AssessorSectionStatus.Pass;
            _sequences.Add(new AssessorSequence { SequenceNumber =  1, Sections = new List<AssessorSection> { new AssessorSection { SectionNumber = 2 } } });
            _outcomes.Add(new PageReviewOutcome { SequenceNumber = 1, SectionNumber = 2,Status = expectedStatus });

            var result = await _orchestrator.GetOverviewViewModel(new GetApplicationOverviewRequest(_applicationId, _userId));

            AssertCommonProperties(result);
            Assert.AreEqual(result.Sequences.First().Sections.First().Status, expectedStatus);
            Assert.IsTrue(result.IsReadyForModeration);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenThereAreSavedOutcomesAndTheyHaveNoStatus_ThenTheOutcomesAreReturnedAndTheApplicationIsNotReadyForModeration()
        {
            var expectedStatus = "";
            _sequences.Add(new AssessorSequence { SequenceNumber = 1, Sections = new List<AssessorSection> { new AssessorSection { SectionNumber = 2 } } });
            _outcomes.Add(new PageReviewOutcome { SequenceNumber = 1, SectionNumber = 2, Status = expectedStatus });

            var result = await _orchestrator.GetOverviewViewModel(new GetApplicationOverviewRequest(_applicationId, _userId));

            AssertCommonProperties(result);
            Assert.AreEqual(result.Sequences.First().Sections.First().Status, expectedStatus);
            Assert.IsFalse(result.IsReadyForModeration);
        }
    }
}
