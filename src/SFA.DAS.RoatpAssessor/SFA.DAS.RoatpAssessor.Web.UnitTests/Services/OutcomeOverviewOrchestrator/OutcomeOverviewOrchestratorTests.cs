using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.OutcomeOverviewOrchestrator
{
    [TestFixture]
    public class OutcomeOverviewOrchestratorTests
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
            var expectedStatus = SectionStatus.Pass;
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
