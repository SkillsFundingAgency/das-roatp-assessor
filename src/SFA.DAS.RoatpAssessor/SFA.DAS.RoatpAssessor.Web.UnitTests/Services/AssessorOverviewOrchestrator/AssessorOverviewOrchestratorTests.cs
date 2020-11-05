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
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.AssessorOverviewOrchestrator
{
    [TestFixture]
    public class AssessorOverviewOrchestratorTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applicationApiClient;
        private Mock<IRoatpAssessorApiClient> _assessorApiClient;
        private Web.Services.AssessorOverviewOrchestrator _orchestrator;
        private string _userId => _user.UserId();
        private Apply _application;
        private Contact _contact;
        private List<AssessorSequence> _sequences;
        private List<AssessorPageReviewOutcome> _outcomes;


        [SetUp]
        public void SetUp()
        {
            _applicationApiClient = new Mock<IRoatpApplicationApiClient>();
            _assessorApiClient = new Mock<IRoatpAssessorApiClient>();
            _orchestrator = new Web.Services.AssessorOverviewOrchestrator(_applicationApiClient.Object, _assessorApiClient.Object);

            _application = new Apply { ApplicationId = _applicationId, OrganisationId = Guid.NewGuid(), Status = "Status", Assessor2UserId = _userId, Assessor2ReviewStatus = AssessorReviewStatus.InProgress };
            _contact = new Contact { Email = "email@address.com" };
            _sequences = new List<AssessorSequence>();
            _outcomes = new List<AssessorPageReviewOutcome>();

            _applicationApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(_application);
            _applicationApiClient.Setup(x => x.GetContactForApplication(_applicationId)).ReturnsAsync(_contact);
            _assessorApiClient.Setup(x => x.GetAssessorSequences(_applicationId)).ReturnsAsync(_sequences);
            _assessorApiClient.Setup(x => x.GetAllAssessorPageReviewOutcomes(_applicationId, _userId)).ReturnsAsync(_outcomes);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenApplicationNotFound_ThenNoViewModelIsReturned()
        {
            var applicationId = Guid.NewGuid();
            _applicationApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync((Apply)null);

            var result = await _orchestrator.GetOverviewViewModel(new GetAssessorOverviewRequest(applicationId, "userId"));

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenApplicationContactNotFound_ThenNoViewModelIsReturned()
        {
            var applicationId = Guid.NewGuid();
            _applicationApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            _applicationApiClient.Setup(x => x.GetContactForApplication(applicationId)).ReturnsAsync((Contact)null);

            var result = await _orchestrator.GetOverviewViewModel(new GetAssessorOverviewRequest(applicationId, "userId"));

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenSequencesNotFound_ThenNoViewModelIsReturned()
        {
            var applicationId = Guid.NewGuid();
            _applicationApiClient.Setup(x => x.GetApplication(applicationId)).ReturnsAsync(new Apply { ApplicationId = applicationId });
            _applicationApiClient.Setup(x => x.GetContactForApplication(applicationId)).ReturnsAsync(new Contact());
            _assessorApiClient.Setup(x => x.GetAssessorSequences(applicationId)).ReturnsAsync((List<AssessorSequence>)null);

            var result = await _orchestrator.GetOverviewViewModel(new GetAssessorOverviewRequest(applicationId, "userId"));

            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenThereAreNoSavedOutcomes_ThenTheApplicationIsNotReadyForModeration()
        {
            var result = await _orchestrator.GetOverviewViewModel(new GetAssessorOverviewRequest(_applicationId, _userId));

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
            var expectedStatus = SectionStatus.Pass;
            _sequences.Add(new AssessorSequence { SequenceNumber = 1, Sections = new List<AssessorSection> { new AssessorSection { SectionNumber = 2 } } });
            _outcomes.Add(new AssessorPageReviewOutcome { SequenceNumber = 1, SectionNumber = 2, Status = expectedStatus });

            var result = await _orchestrator.GetOverviewViewModel(new GetAssessorOverviewRequest(_applicationId, _userId));

            AssertCommonProperties(result);
            Assert.AreEqual(result.Sequences.First().Sections.First().Status, expectedStatus);
            Assert.IsTrue(result.IsReadyForModeration);
        }

        [Test]
        public async Task GetOverviewViewModel_WhenThereAreSavedOutcomesAndTheyHaveNoStatus_ThenTheOutcomesAreReturnedAndTheApplicationIsNotReadyForModeration()
        {
            var expectedStatus = "";
            _sequences.Add(new AssessorSequence { SequenceNumber = 1, Sections = new List<AssessorSection> { new AssessorSection { SectionNumber = 2 } } });
            _outcomes.Add(new AssessorPageReviewOutcome { SequenceNumber = 1, SectionNumber = 2, Status = expectedStatus });

            var result = await _orchestrator.GetOverviewViewModel(new GetAssessorOverviewRequest(_applicationId, _userId));

            AssertCommonProperties(result);
            Assert.AreEqual(result.Sequences.First().Sections.First().Status, expectedStatus);
            Assert.IsFalse(result.IsReadyForModeration);
        }
    }
}
