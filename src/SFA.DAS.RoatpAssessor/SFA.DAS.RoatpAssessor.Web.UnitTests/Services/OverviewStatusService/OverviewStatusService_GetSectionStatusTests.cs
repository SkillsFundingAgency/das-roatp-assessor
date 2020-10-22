using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.OverviewStatusService
{
    [TestFixture]
    public class OverviewStatusService_GetSectionStatusTests
    {
        private const int SequenceNumber = 1;
        private const int SectionNumber = 2;
        const string Status = "status goes here";
        private List<ModeratorPageReviewOutcome> outcomes;

        [SetUp]
        public void Setup()
        {
            outcomes = new List<ModeratorPageReviewOutcome>();
        }

        [Test]
        public void GetSectionStatus_WhenThereAreNoMatchingOutcomes()
        {
            var result = Web.Services.OverviewStatusService.GetSectionStatus(outcomes, SequenceNumber, SectionNumber);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetSectionStatus_WhenThereAreNullMatchingOutcomes()
        {
            var result = Web.Services.OverviewStatusService.GetSectionStatus(null, SequenceNumber, SectionNumber);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetSectionStatus_WhenThereAreNoMatchingOutcomesWithMatchingSequenceDetails()
        {
            outcomes.Add(new ModeratorPageReviewOutcome {SequenceNumber = 2, SectionNumber = SectionNumber, Status = Status});
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = 3, SectionNumber = SectionNumber, Status = $"{Status}2"});
            var result = Web.Services.OverviewStatusService.GetSectionStatus(outcomes, SequenceNumber, SectionNumber);
            Assert.IsEmpty(result);
        }


        [Test]
        public void GetSectionStatus_WhenThereAreNoMatchingOutcomesWithMatchingSectionDetails()
        {
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = 1 , Status = Status});
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = 3 , Status = $"{Status}2"});
            var result = Web.Services.OverviewStatusService.GetSectionStatus(outcomes, SequenceNumber, SectionNumber);
            Assert.IsEmpty(result);
        }

        [Test]
        public void GetSectionStatus_WhenThereAreOneAndOneOnlyMatchingOutcomes()
        {
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = Status});
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = 5, Status = $"{Status}2" });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = 5, SectionNumber = SectionNumber, Status = $"{Status}5" });
            var expectedStatus = Web.Services.OverviewStatusService.GetSectionStatus(outcomes, SequenceNumber, SectionNumber);
            Assert.AreEqual(expectedStatus,Status);
        }

        [Test]
        public void GetSectionStatus_WhenThereAreSeveralMatchingOutcomeButNoStatusSetInThem()
        {
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber});
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber });
            var expectedStatus = Web.Services.OverviewStatusService.GetSectionStatus(outcomes, SequenceNumber, SectionNumber);
            Assert.IsNull(expectedStatus);
        }

        [Test]
        public void GetSectionStatus_WhenThereAreSeveralMatchingOutcomesThatAllPass()
        {
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass});
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });
            var expectedStatus = Web.Services.OverviewStatusService.GetSectionStatus(outcomes, SequenceNumber, SectionNumber);
            Assert.AreEqual(expectedStatus, ModeratorSectionStatus.Pass);
        }

        [Test]
        public void GetSectionStatus_WhenThereAreSeveralMatchingOutcomesThat3Pass1Fail()
        {
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Fail });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });

            var expectedStatus = Web.Services.OverviewStatusService.GetSectionStatus(outcomes, SequenceNumber, SectionNumber);
            Assert.AreEqual(expectedStatus, "1 Fail Out Of 4");
        }

        [Test]
        public void GetSectionStatus_WhenThereAreSeveralMatchingOutcomesThat2Pass2Fail()
        {
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Fail });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Fail });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });

            var expectedStatus = Web.Services.OverviewStatusService.GetSectionStatus(outcomes, SequenceNumber, SectionNumber);
            Assert.AreEqual(expectedStatus, "2 Fails Out Of 4");
        }

        [Test]
        public void GetSectionStatus_WhenThereAreSeveralMatchingOutcomesThat2Pass1Fail1NotSet()
        {
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Pass });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Fail });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber, Status = ModeratorSectionStatus.Fail });
            outcomes.Add(new ModeratorPageReviewOutcome { SequenceNumber = SequenceNumber, SectionNumber = SectionNumber });

            var expectedStatus = Web.Services.OverviewStatusService.GetSectionStatus(outcomes, SequenceNumber, SectionNumber);
            Assert.AreEqual(expectedStatus, ModeratorSectionStatus.InProgress);
        }
    }
}
