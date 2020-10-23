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
                    Status = statusOne,
                    SequenceNumber = SequenceIds.DeliveringApprenticeshipTraining,
                    SectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees
                },
                new ModeratorPageReviewOutcome
                {
                    Status = statusTwo,
                    SequenceNumber = SequenceIds.DeliveringApprenticeshipTraining,
                    SectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees
                },
                new ModeratorPageReviewOutcome
                {
                    Status = statusThree,
                    SequenceNumber = SequenceIds.DeliveringApprenticeshipTraining,
                    SectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees
                }
            };

            var sectionStatus = Web.Services.OverviewStatusService.GetSectorsSectionStatus(sectionPageReviewOutcomes);
            Assert.AreEqual(statusExpected, sectionStatus);
        }
    }
}
