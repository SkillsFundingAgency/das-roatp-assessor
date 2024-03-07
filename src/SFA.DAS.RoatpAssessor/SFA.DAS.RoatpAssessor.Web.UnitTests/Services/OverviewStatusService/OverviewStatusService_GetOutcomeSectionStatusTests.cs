﻿using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Domain;
using System.Collections.Generic;
using StatusService = SFA.DAS.RoatpAssessor.Web.Services.OverviewStatusService;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.OverviewStatusService
{
    [TestFixture]
    public class OverviewStatusService_GetOutcomeSectionStatusTests
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

            var result = StatusService.GetOutcomeSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.IsEmpty(result);
        }

        [Test]
        public void When_Outcomes_IsEmpty_Returns_Empty()
        {
            _outcomes = new List<ClarificationPageReviewOutcome>();

            var result = StatusService.GetOutcomeSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.IsEmpty(result);
        }

        [Test]
        public void When_Outcomes_DoNotMatch_Requested_SequenceNumber_Returns_Empty()
        {
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber + 1, SectionNumber = _sectionNumber });
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber + 2, SectionNumber = _sectionNumber });

            var result = StatusService.GetOutcomeSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.IsEmpty(result);
        }


        [Test]
        public void When_Outcomes_DoNotMatch_Requested_SectionNumber_Returns_Empty()
        {
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber + 1 });
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber + 2 });

            var result = StatusService.GetOutcomeSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.IsEmpty(result);
        }

        [TestCase(null, null)]
        [TestCase(ClarificationPageReviewStatus.Pass, ClarificationPageReviewStatus.Pass)]
        [TestCase(ClarificationPageReviewStatus.Fail, ClarificationPageReviewStatus.Fail)]
        [TestCase(ClarificationPageReviewStatus.InProgress, ClarificationPageReviewStatus.InProgress)]
        public void When_Single_Outcome_Returns_Expected_Status(string pageReviewStatus, string expectedStatus)
        {
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = pageReviewStatus });

            var result = StatusService.GetOutcomeSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.AreEqual(expectedStatus, result);
        }

        [Test]
        public void When_Several_Matching_Outcomes_And_All_Empty_Returns_Empty()
        {
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = string.Empty });
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = string.Empty });
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = string.Empty });

            var result = StatusService.GetOutcomeSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.IsEmpty(result);
        }

        [Test]
        public void When_Several_Matching_Outcomes_And_All_Pass_Returns_Pass()
        {
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = ClarificationPageReviewStatus.Pass });
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = ClarificationPageReviewStatus.Pass });
            _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = ClarificationPageReviewStatus.Pass });

            var result = StatusService.GetOutcomeSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.AreEqual(SectionStatus.Pass, result);
        }

        [TestCase(2, 1, false, SectionStatus.Fail)]
        [TestCase(2, 1, true, SectionStatus.Fail)]
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

            var result = StatusService.GetOutcomeSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
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

            var result = StatusService.GetOutcomeSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
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

            var result = StatusService.GetOutcomeSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.AreEqual(SectionStatus.InProgress.ToLower(), result.ToLower());
        }

        [TestCase(2, 1, false, SectionStatus.Fail)]
        [TestCase(2, 1, true, SectionStatus.Fail)]
        [TestCase(1, 2, false, SectionStatus.Fail)]
        [TestCase(1, 2, true, SectionStatus.Fail)]
        [TestCase(0, 3, false, SectionStatus.Fail)]
        [TestCase(0, 3, true, SectionStatus.Fail)]
        [TestCase(3, 0, false, SectionStatus.Pass)]
        [TestCase(3, 0, true, SectionStatus.Pass)]
        public void When_Several_Matching_Outcomes_And_Moderation_PassedOrFailed_And_No_Clarification_Process_Returns_Expected_Status(int passOutcomes, int failOutcomes, bool isSectorsSection, string expectedStatus)
        {
            if (isSectorsSection)
            {
                _sequenceNumber = SequenceIds.DeliveringApprenticeshipTraining;
                _sectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees;
            }

            for (int p = 0; p < passOutcomes; p++)
            {
                _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = null, ModeratorReviewStatus = ModeratorPageReviewStatus.Pass });
            }

            for (int f = 0; f < failOutcomes; f++)
            {
                _outcomes.Add(new ClarificationPageReviewOutcome { SequenceNumber = _sequenceNumber, SectionNumber = _sectionNumber, Status = null, ModeratorReviewStatus = ModeratorPageReviewStatus.Fail });
            }

            var result = StatusService.GetOutcomeSectionStatus(_outcomes, _sequenceNumber, _sectionNumber);
            Assert.AreEqual(expectedStatus, result);
        }
    }
}
