using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Domain;
using StatusService = SFA.DAS.RoatpAssessor.Web.Services.OverviewStatusService;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.OverviewStatusService
{
    [TestFixture]
    public class OverviewStatusService_GetClarificationSectionStatusTests
    {
        private int _sequenceNumber;
        private int _sectionNumber;
        private List<ClarificationPageReviewOutcome> _outcomes;

        [SetUp]
        public void Setup()
        {
            _sequenceNumber = 1;
            _sectionNumber = 1;
            _outcomes = new List<ClarificationPageReviewOutcome>();
        }

        [Test]
        public void When_Outcomes_IsNull_Then_returns_Empty()
        {
            _outcomes = null;

            var result = StatusService.GetClarificationSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.IsEmpty(result);
        }

        [Test]
        public void When_Outcomes_IsEmpty_Returns_Empty()
        {
            _outcomes = new List<ClarificationPageReviewOutcome>();

            var result = StatusService.GetClarificationSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.IsEmpty(result);
        }

        [Test]
        public void When_Outcomes_DoNotMatch_Requested_SequenceNumber_Returns_Empty()
        {
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber + 1, SectionNumber = _sectionNumber, ModeratorReviewStatus = ModeratorPageReviewStatus.Fail });
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber + 2, SectionNumber = _sectionNumber, ModeratorReviewStatus = ModeratorPageReviewStatus.Fail });

            var result = StatusService.GetClarificationSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.IsEmpty(result);
        }


        [Test]
        public void When_Outcomes_DoNotMatch_Requested_SectionNumber_Returns_Empty()
        {
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber + 1, ModeratorReviewStatus = ModeratorPageReviewStatus.Fail });
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber + 2, ModeratorReviewStatus = ModeratorPageReviewStatus.Fail });

            var result = StatusService.GetClarificationSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.IsEmpty(result);
        }

        [TestCase(null, SectionStatus.Clarification)]
        [TestCase(ClarificationPageReviewStatus.Pass, SectionStatus.Pass)]
        [TestCase(ClarificationPageReviewStatus.Fail, SectionStatus.Fail)]
        [TestCase(ClarificationPageReviewStatus.InProgress, SectionStatus.InProgress)]
        public void When_Single_Outcome_Returns_Expected_Status(string pageReviewStatus, string expectedStatus)
        {
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = pageReviewStatus, ModeratorReviewStatus = ModeratorPageReviewStatus.Fail });
            _outcomes.ForEach(o => o.ModeratorReviewStatus = ModeratorPageReviewStatus.Fail);

            var result = StatusService.GetClarificationSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.AreEqual(expectedStatus, result);
        }

        [Test]
        public void When_Several_Matching_Outcomes_And_All_Empty_Returns_Clarification()
        {
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = string.Empty, ModeratorReviewStatus = ModeratorPageReviewStatus.Fail });
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = string.Empty, ModeratorReviewStatus = ModeratorPageReviewStatus.Fail });
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = string.Empty, ModeratorReviewStatus = ModeratorPageReviewStatus.Fail });
            _outcomes.ForEach(o => o.ModeratorReviewStatus = ModeratorPageReviewStatus.Fail);

            var result = StatusService.GetClarificationSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.AreEqual(SectionStatus.Clarification, result);
        }

        [Test]
        public void When_Several_Matching_Outcomes_And_All_Pass_Returns_Pass()
        {
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = ClarificationPageReviewStatus.Pass });
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = ClarificationPageReviewStatus.Pass });
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = ClarificationPageReviewStatus.Pass });
            _outcomes.ForEach(o => o.ModeratorReviewStatus = ModeratorPageReviewStatus.Fail);

            var result = StatusService.GetClarificationSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.AreEqual(SectionStatus.Pass, result);
        }

        [TestCase(2, 1, false, SectionStatus.Fail)]
        [TestCase(1, 2, false, SectionStatus.Fail)]
        [TestCase(1, 2, true, SectionStatus.Fail)]
        [TestCase(0, 3, false, SectionStatus.Fail)]
        [TestCase(0, 3, true, SectionStatus.Fail)]
        public void When_Several_Matching_Outcomes_And_All_Pass_Or_Fail_Returns_Expected_Status(int passOutcomes, int failOutcomes, bool isSectorsSection, string expectedStatus)
        {
            if (isSectorsSection)
            {
                _sequenceNumber = SequenceIds.DeliveringApprenticeshipTraining;
                _sectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees;
            }

            for (int p = 0; p < passOutcomes; p++)
            {
                _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = ClarificationPageReviewStatus.Pass });
            }

            for (int f = 0; f < failOutcomes; f++)
            {
                _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = ClarificationPageReviewStatus.Fail });
            }

            var result = StatusService.GetClarificationSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.AreEqual(expectedStatus, result);
        }

        [TestCase(0, 1, 1)]
        [TestCase(1, 0, 1)]
        [TestCase(1, 1, 1)]
        public void When_Several_Matching_Outcomes_And_Some_Are_Not_Set_Returns_InProgress(int passOutcomes, int failOutcomes, int notSetOutcomes)
        {
            for (int p = 0; p < passOutcomes; p++)
            {
                _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = ClarificationPageReviewStatus.Pass });
            }

            for (int f = 0; f < failOutcomes; f++)
            {
                _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = ClarificationPageReviewStatus.Fail });
            }

            for (int n = 0; n < notSetOutcomes; n++)
            {
                _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = null });
            }

            var result = StatusService.GetClarificationSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.AreEqual(SectionStatus.InProgress, result);
        }

        [TestCase(0, 0, 1)]
        [TestCase(0, 0, 2)]
        [TestCase(0, 1, 1)]
        [TestCase(1, 0, 1)]
        [TestCase(1, 1, 1)]
        public void When_Several_Matching_Outcomes_And_Some_Are_InProgress_Returns_InProgress(int passOutcomes, int failOutcomes, int inProgressOutcomes)
        {
            for (int p = 0; p < passOutcomes; p++)
            {
                _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = ClarificationPageReviewStatus.Pass });
            }

            for (int f = 0; f < failOutcomes; f++)
            {
                _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = ClarificationPageReviewStatus.Fail });
            }

            for (int i = 0; i < inProgressOutcomes; i++)
            {
                _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = ClarificationPageReviewStatus.InProgress });
            }

            var result = StatusService.GetClarificationSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.AreEqual(SectionStatus.InProgress, result);
        }
    }
}
