using System.Collections.Generic;
using System.Linq;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Domain;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public static class OverviewStatusService
    {
        public static string GetAssessorSectionStatus(List<AssessorPageReviewOutcome> pageReviewOutcomes, int sequenceNumber, int sectionNumber)
        {
            var sectionPageReviewOutcomes = pageReviewOutcomes?.Where(p =>
                p.SequenceNumber == sequenceNumber &&
                p.SectionNumber == sectionNumber).ToList();

            var outcomes = sectionPageReviewOutcomes?.Select(o => o.Status).ToList();
            var isSectorsSection = sequenceNumber == SequenceIds.DeliveringApprenticeshipTraining && sectionNumber == SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees;

            return GetStatusFromOutcomes(outcomes, !isSectorsSection);
        }

        public static string GetModerationSectionStatus(List<ModeratorPageReviewOutcome> pageReviewOutcomes, int sequenceNumber, int sectionNumber)
        {
            var sectionPageReviewOutcomes = pageReviewOutcomes?.Where(p =>
                p.SequenceNumber == sequenceNumber &&
                p.SectionNumber == sectionNumber).ToList();

            var outcomes = sectionPageReviewOutcomes?.Select(o => o.Status).ToList();
            var isSectorsSection = sequenceNumber == SequenceIds.DeliveringApprenticeshipTraining && sectionNumber == SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees;

            return GetStatusFromOutcomes(outcomes, !isSectorsSection);
        }

        public static string GetClarificationSectionStatus(List<ClarificationPageReviewOutcome> pageReviewOutcomes, int sequenceNumber, int sectionNumber)
        {
            var sectionPageReviewOutcomes = pageReviewOutcomes?.Where(p =>
                p.SequenceNumber == sequenceNumber &&
                p.SectionNumber == sectionNumber).ToList();

            var failedModeratorPages = sectionPageReviewOutcomes?.Where(p => p.ModeratorReviewStatus == ModeratorPageReviewStatus.Fail);

            if (failedModeratorPages?.Any() is true && failedModeratorPages.All(p => string.IsNullOrEmpty(p.Status)))
            { 
                return SectionStatus.Clarification;
            }
            else
            {
                var outcomes = sectionPageReviewOutcomes?.Select(o => o.Status).ToList();
                return GetStatusFromOutcomes(outcomes, false);
            }
        }

        public static string GetOutcomeSectionStatus(List<ClarificationPageReviewOutcome> pageReviewOutcomes, int sequenceNumber, int sectionNumber)
        {
            var sectionPageReviewOutcomes = pageReviewOutcomes?.Where(p =>
                p.SequenceNumber == sequenceNumber &&
                p.SectionNumber == sectionNumber).ToList();

            var outcomes = sectionPageReviewOutcomes?.Select(o => o.Status ?? o.ModeratorReviewStatus).ToList();

            return GetStatusFromOutcomes(outcomes, false);
        }

        private static string GetStatusFromOutcomes(List<string> outcomes, bool expandFailCount)
        {
            var passStatuses = new List<string> { AssessorPageReviewStatus.Pass, ModeratorPageReviewStatus.Pass, ClarificationPageReviewStatus.Pass };
            var failStatuses = new List<string> { AssessorPageReviewStatus.Fail, ModeratorPageReviewStatus.Fail, ClarificationPageReviewStatus.Fail };

            string status;

            if(outcomes is null || outcomes.Count == 0)
            {
                status = string.Empty;
            }
            else if (outcomes.Count == 1)
            {
                status = outcomes[0];
            }
            else
            {
                if (outcomes.All(o => string.IsNullOrEmpty(o)))
                {
                    status = string.Empty;
                }
                else if (outcomes.All(o => passStatuses.Contains(o)))
                {
                    status = SectionStatus.Pass;
                }
                else if (outcomes.All(o => passStatuses.Contains(o) || failStatuses.Contains(o)))
                {
                    if (expandFailCount)
                    {
                        var outcomesCount = outcomes.Count;
                        var failCount = outcomes.Count(o => SectionStatus.Fail.Equals(o));
                        var pluarlisedFailsOutOf = failCount == 1 ? SectionStatus.FailOutOf : SectionStatus.FailsOutOf;

                        status = $"{failCount} {pluarlisedFailsOutOf} {outcomesCount}";
                    }
                    else
                    {
                        status = SectionStatus.Fail;
                    }
                }
                else
                {
                    status = SectionStatus.InProgress;
                }
            }

            return status;
        }
    }
}
