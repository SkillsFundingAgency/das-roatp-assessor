using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.ModeratorOverviewOrchestrator
{
    [TestFixture]
    public class ModeratorOverviewOrchestratorTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applicationApiClient;
        private Mock<IRoatpModerationApiClient> _moderationApiClient;
        private Web.Services.ModeratorOverviewOrchestrator _orchestrator;
        private string _userId => _user.UserId();
        private string _userDisplayName => _user.UserDisplayName();
        private Apply _application;
        private Contact _contact;
        private List<ModeratorSequence> _sequences;
        private List<ModeratorPageReviewOutcome> _outcomes;


        [SetUp]
        public void SetUp()
        {
            _applicationApiClient = new Mock<IRoatpApplicationApiClient>();
            _moderationApiClient = new Mock<IRoatpModerationApiClient>();
            _orchestrator = new Web.Services.ModeratorOverviewOrchestrator(_applicationApiClient.Object, _moderationApiClient.Object);

            _application = new Apply
            {
                ApplicationId = _applicationId,
                ModerationStatus = ModerationStatus.New,
                Assessor1ReviewStatus = AssessorReviewStatus.Approved,
                Assessor1UserId = _userId,
                Assessor1Name = _userDisplayName,
                Assessor2ReviewStatus = AssessorReviewStatus.Approved,
                Assessor2UserId = $"{ _userId }-2",
                Assessor2Name = $"{ _userDisplayName }-2"
            };

            _contact = new Contact { Email = "email@address.com" };
            _sequences = new List<ModeratorSequence>();
            _outcomes = new List<ModeratorPageReviewOutcome>();

            _applicationApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(_application);
            _applicationApiClient.Setup(x => x.GetContactForApplication(_applicationId)).ReturnsAsync(_contact);
            _moderationApiClient.Setup(x => x.GetModeratorSequences(_applicationId)).ReturnsAsync(_sequences);
            _moderationApiClient.Setup(x => x.GetAllModeratorPageReviewOutcomes(_applicationId, _userId)).ReturnsAsync(_outcomes);
        }

        [TestCase(null)]
        [TestCase(ModeratorPageReviewStatus.Pass)]
        [TestCase(ModeratorPageReviewStatus.Fail)]
        [TestCase(ModeratorPageReviewStatus.InProgress)]
        public void GetSectionStatus_Single_PageReviewOutcome(string status)
        {
            const int sequenceNumber = 1;
            const int sectionNumber = 1;

            List<ModeratorPageReviewOutcome> sectionPageReviewOutcomes = new List<ModeratorPageReviewOutcome>
            {
                new ModeratorPageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    SequenceNumber = sequenceNumber,
                    SectionNumber = sectionNumber,
                    Status = status
                }
            };

            var sectionStatus = _orchestrator.GetSectionStatus(sectionPageReviewOutcomes, sequenceNumber, sectionNumber);
            Assert.AreSame(status, sectionStatus);
        }

        [TestCase(null, null, null, null)]
        [TestCase(ModeratorPageReviewStatus.Pass, null, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.InProgress, null, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Fail, null, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.InProgress, ModeratorPageReviewStatus.Pass, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.InProgress, ModeratorPageReviewStatus.Fail, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Pass, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Fail, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Fail, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Pass, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.InProgress, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.InProgress, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.InProgress, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.InProgress, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Pass, ModeratorSectionStatus.Pass)]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Pass, "1 " + ModeratorSectionStatus.FailOutOf + " 3")]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Pass, "1 " + ModeratorSectionStatus.FailOutOf + " 3")]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Fail, "1 " + ModeratorSectionStatus.FailOutOf + " 3")]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Fail, "2 " + ModeratorSectionStatus.FailsOutOf + " 3")]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Fail, "2 " + ModeratorSectionStatus.FailsOutOf + " 3")]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Pass, "2 " + ModeratorSectionStatus.FailsOutOf + " 3")]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Fail, "3 " + ModeratorSectionStatus.FailsOutOf + " 3")]
        public void GetSectionStatus_Multiple_PageReviewOutcomes(string statusOne, string statusTwo, string statusThree, string statusExpected)
        {
            const int sequenceNumber = 1;
            const int sectionNumber = 1;

            List<ModeratorPageReviewOutcome> sectionPageReviewOutcomes = new List<ModeratorPageReviewOutcome>
            {
                new ModeratorPageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    SequenceNumber = sequenceNumber,
                    SectionNumber = sectionNumber,
                    Status = statusOne
                },
                new ModeratorPageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    SequenceNumber = sequenceNumber,
                    SectionNumber = sectionNumber,
                    Status = statusTwo
                },
                new ModeratorPageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    SequenceNumber = sequenceNumber,
                    SectionNumber = sectionNumber,
                    Status = statusThree
                }
            };

            var sectionStatus = _orchestrator.GetSectionStatus(sectionPageReviewOutcomes, sequenceNumber, sectionNumber);
            Assert.AreEqual(statusExpected, sectionStatus);
        }

        [TestCase(null, null, null, null)]
        [TestCase(ModeratorPageReviewStatus.Pass, null, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.InProgress, null, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Fail, null, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.InProgress, ModeratorPageReviewStatus.Pass, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.InProgress, ModeratorPageReviewStatus.Fail, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Pass, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Fail, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Fail, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Pass, null, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.InProgress, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.InProgress, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.InProgress, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.InProgress, ModeratorSectionStatus.InProgress)]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Pass, ModeratorSectionStatus.Pass)]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Pass, ModeratorSectionStatus.Fail)]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Pass, ModeratorSectionStatus.Fail)]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Fail, ModeratorSectionStatus.Fail)]
        [TestCase(ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Fail, ModeratorSectionStatus.Fail)]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Pass, ModeratorPageReviewStatus.Fail, ModeratorSectionStatus.Fail)]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Pass, ModeratorSectionStatus.Fail)]
        [TestCase(ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Fail, ModeratorPageReviewStatus.Fail, ModeratorSectionStatus.Fail)]
        public void GetSectorsSectionStatus(string statusOne, string statusTwo, string statusThree, string statusExpected)
        {
            List<ModeratorPageReviewOutcome> sectionPageReviewOutcomes = new List<ModeratorPageReviewOutcome>
            {
                new ModeratorPageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    Status = statusOne,
                    SequenceNumber = SequenceIds.DeliveringApprenticeshipTraining,
                    SectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees
                },
                new ModeratorPageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    Status = statusTwo,
                    SequenceNumber = SequenceIds.DeliveringApprenticeshipTraining,
                    SectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees
                },
                new ModeratorPageReviewOutcome
                {
                    ApplicationId = _applicationId,
                    Status = statusThree,
                    SequenceNumber = SequenceIds.DeliveringApprenticeshipTraining,
                    SectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees
                }
            };

            var sectionStatus = _orchestrator.GetSectorsSectionStatus(sectionPageReviewOutcomes);
            Assert.AreEqual(statusExpected, sectionStatus);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenApplicationNotFound_ThenNoViewModelIsReturned()
        {
            var applicationId = Guid.NewGuid();
            _applicationApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync((Apply)null);

            var result = await _orchestrator.GetOverviewViewModel(new GetModeratorOverviewRequest(applicationId, "userId"));

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenApplicationContactNotFound_ThenNoViewModelIsReturned()
        {
            var applicationId = Guid.NewGuid();
            _applicationApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            _applicationApiClient.Setup(x => x.GetContactForApplication(applicationId)).ReturnsAsync((Contact)null);

            var result = await _orchestrator.GetOverviewViewModel(new GetModeratorOverviewRequest(applicationId, "userId"));

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenSequencesNotFound_ThenNoViewModelIsReturned()
        {
            var applicationId = Guid.NewGuid();
            _applicationApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            _applicationApiClient.Setup(x => x.GetContactForApplication(applicationId)).ReturnsAsync(new Contact());
            _moderationApiClient.Setup(x => x.GetModeratorSequences(applicationId)).ReturnsAsync((List<ModeratorSequence>)null);

            var result = await _orchestrator.GetOverviewViewModel(new GetModeratorOverviewRequest(applicationId, "userId"));

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenThereAreNoSavedOutcomes_ThenTheApplicationIsNotReadyForModerationConfirmation()
        {
            var result = await _orchestrator.GetOverviewViewModel(new GetModeratorOverviewRequest(_applicationId, _userId));

            AssertCommonProperties(result);
            Assert.IsFalse(result.IsReadyForModeratorConfirmation);
        }

        private void AssertCommonProperties(ModeratorApplicationViewModel result)
        {
            Assert.AreEqual(_application.Id, result.Id);
            Assert.AreEqual(_application.ApplicationId, result.ApplicationId);
            Assert.AreEqual(_application.OrganisationId, result.OrgId);
            Assert.AreEqual(_contact.Email, result.ApplicantEmailAddress);
            Assert.AreEqual(_application.ApplicationStatus, result.ApplicationStatus);
            Assert.AreEqual(_application.ModerationStatus, result.ModerationStatus);
            Assert.AreEqual(_application.Assessor1Name, result.Assessor1Name);
            Assert.AreEqual(_application.Assessor2Name, result.Assessor2Name);
            Assert.AreSame(_sequences, result.Sequences);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenThereAreSavedOutcomesAndTheyAllPass_ThenTheOutcomesAreReturnedAndTheApplicationIsReadyForModerationConfirmation()
        {
            var expectedStatus = ModeratorSectionStatus.Pass;
            _sequences.Add(new ModeratorSequence { SequenceNumber = 1, Sections = new List<ModeratorSection> { new ModeratorSection { SectionNumber = 2 } } });
            _outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = 1, SectionNumber = 2, Status = expectedStatus });

            var result = await _orchestrator.GetOverviewViewModel(new GetModeratorOverviewRequest(_applicationId, _userId));

            AssertCommonProperties(result);
            Assert.AreEqual(result.Sequences.First().Sections.First().Status, expectedStatus);
            Assert.IsTrue(result.IsReadyForModeratorConfirmation);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenThereAreSavedOutcomesAndTheyHaveNoStatus_ThenTheOutcomesAreReturnedAndTheApplicationIsNotReadyForModerationConfirmation()
        {
            var expectedStatus = "";
            _sequences.Add(new ModeratorSequence { SequenceNumber = 1, Sections = new List<ModeratorSection> { new ModeratorSection { SectionNumber = 2 } } });
            _outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = 1, SectionNumber = 2, Status = expectedStatus });

            var result = await _orchestrator.GetOverviewViewModel(new GetModeratorOverviewRequest(_applicationId, _userId));

            AssertCommonProperties(result);
            Assert.AreEqual(result.Sequences.First().Sections.First().Status, expectedStatus);
            Assert.IsFalse(result.IsReadyForModeratorConfirmation);
        }
    }
}
