using System.Collections.Generic;
using System.Linq;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Domain;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public static class OverviewStatusService
    {
        public static string GetSectionStatus(List<ModeratorPageReviewOutcome> pageReviewOutcomes, int sequenceNumber, int sectionNumber)
        {
            var sectionPageReviewOutcomes = pageReviewOutcomes?.Where(p =>
                p.SequenceNumber == sequenceNumber &&
                p.SectionNumber == sectionNumber).ToList();

            var sectionStatus = string.Empty;

            if (sectionPageReviewOutcomes != null && sectionPageReviewOutcomes.Any())
            {
                if (sectionPageReviewOutcomes.Count.Equals(1))
                {
                    sectionStatus = sectionPageReviewOutcomes[0].Status;
                }
                else
                {
                    if (sectionPageReviewOutcomes.All(p => string.IsNullOrEmpty(p.Status)))
                    {
                        sectionStatus = null;
                    }
                    else if (sectionPageReviewOutcomes.All(x => x.Status == ModeratorPageReviewStatus.Pass))
                    {
                        sectionStatus = ModeratorSectionStatus.Pass;
                    }
                    else if (sectionPageReviewOutcomes.All(p =>
                        p.Status == ModeratorPageReviewStatus.Pass || p.Status == ModeratorPageReviewStatus.Fail))
                    {
                        var failStatusesCount = sectionPageReviewOutcomes.Count(p => p.Status == ModeratorPageReviewStatus.Fail);
                        var pluarlisedFailsOutOf = failStatusesCount == 1 ? ModeratorSectionStatus.FailOutOf : ModeratorSectionStatus.FailsOutOf;

                        sectionStatus = $"{failStatusesCount} {pluarlisedFailsOutOf} {sectionPageReviewOutcomes.Count}";
                    }
                    else
                    {
                        sectionStatus = ModeratorSectionStatus.InProgress;
                    }
                }
            }

            return sectionStatus;
        }

        public static string GetSectorsSectionStatus(List<ModeratorPageReviewOutcome> pageReviewOutcomes)
        {
            var sectionPageReviewOutcomes = pageReviewOutcomes?.Where(p =>
                p.SequenceNumber == SequenceIds.DeliveringApprenticeshipTraining &&
                p.SectionNumber == SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees).ToList();

            var sectionStatus = string.Empty;

            if (sectionPageReviewOutcomes != null && sectionPageReviewOutcomes.Any())
            {
                if (sectionPageReviewOutcomes.All(p => string.IsNullOrEmpty(p.Status)))
                {
                    sectionStatus = null;
                }
                else if (sectionPageReviewOutcomes.All(p => p.Status == ModeratorPageReviewStatus.Pass))
                {
                    sectionStatus = ModeratorSectionStatus.Pass;
                }
                else if (sectionPageReviewOutcomes.All(p =>
                            p.Status == ModeratorPageReviewStatus.Pass || p.Status == ModeratorPageReviewStatus.Fail))
                {
                    sectionStatus = ModeratorSectionStatus.Fail;
                }
                else
                {
                    sectionStatus = ModeratorSectionStatus.InProgress;
                }
            }

            return sectionStatus;
        }
    }
}
