using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Domain;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.OverviewStatusService
{
    [TestFixture]
    public class OverviewStOverviewStatusService_GetSectorsSectionStatusTestsatusServiceTests
    {
        private  int SequenceNumber = SequenceIds.DeliveringApprenticeshipTraining;
        private  int SectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees;
        const string Status = "status goes here";
        private List<ModeratorPageReviewOutcome> outcomes;

        [SetUp]
        public void Setup()
        {
            outcomes = new List<ModeratorPageReviewOutcome>();
        }

        [Test]
        public void GetSectorsSectionStatus_WhenThereAreNoMatchingOutcomes()
        {
            var result = Web.Services.OverviewStatusService.GetSectorsSectionStatus(outcomes);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetSectorsSectionStatus_WhenThereAreNullMatchingOutcomes()
        {
            var result = Web.Services.OverviewStatusService.GetSectorsSectionStatus(null);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetSectorsSectionStatus_WhenThereAreNoMatchingOutcomesWithMatchingSequenceDetails()
        {
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = 2, SectionNumber = SectionNumber, Status = Status });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = 3, SectionNumber = SectionNumber, Status = $"{Status}2" });
            var result = Web.Services.OverviewStatusService.GetSectorsSectionStatus(outcomes);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetSectorsSectionStatus_WhenThereAreNoMatchingOutcomesWithMatchingSectionDetails()
        {
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = 1, Status = Status });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = 3, Status = $"{Status}2" });
            var result = Web.Services.OverviewStatusService.GetSectorsSectionStatus(outcomes);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetSectorsSectionStatus_WhenThereAreSeveralMatchingOutcomeButNoStatusSetInThem()
        {
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber });
            var expectedStatus = Web.Services.OverviewStatusService.GetSectorsSectionStatus(outcomes);
            Assert.IsNull(expectedStatus);
        }

        [Test]
        public void GetSectorsSectionStatus_WhenThereAreSeveralMatchingOutcomesThatAllPass()
        {
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });
            var expectedStatus = Web.Services.OverviewStatusService.GetSectorsSectionStatus(outcomes);
            Assert.AreEqual(expectedStatus, ModeratorSectionStatus.Pass);
        }

        [Test]
        public void GetSectorsSectionStatus_WhenThereAreSeveralMatchingOutcomesThat3Pass1Fail()
        {
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Fail });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });

            var expectedStatus = Web.Services.OverviewStatusService.GetSectorsSectionStatus(outcomes);
            Assert.AreEqual(expectedStatus,ModeratorSectionStatus.Fail);
        }

        [Test]
        public void GetSectionStatus_WhenThereAreSeveralMatchingOutcomesThat2Pass1Fail1NotSet()
        {
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Fail });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Fail });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber });

            var expectedStatus = Web.Services.OverviewStatusService.GetSectorsSectionStatus(outcomes);
            Assert.AreEqual(expectedStatus, ModeratorSectionStatus.InProgress);
        }
    }
}
