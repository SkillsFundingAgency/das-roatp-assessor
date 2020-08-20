using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class ModeratorOverviewOrchestrator : IModeratorOverviewOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applicationApiClient;
        private readonly IRoatpModerationApiClient _moderationApiClient;

        public ModeratorOverviewOrchestrator(IRoatpApplicationApiClient applicationApiClient, IRoatpModerationApiClient moderationApiClient)
        {
            _applicationApiClient = applicationApiClient;
            _moderationApiClient = moderationApiClient;
        }

        public async Task<ModeratorApplicationViewModel> GetOverviewViewModel(GetModeratorOverviewRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var contact = await _applicationApiClient.GetContactForApplication(request.ApplicationId);
            var sequences = await _moderationApiClient.GetModeratorSequences(request.ApplicationId);

            if (application is null || contact is null || sequences is null)
            {
                return null;
            }

            var viewmodel = new ModeratorApplicationViewModel(application, contact, sequences, request.UserId);

            var savedOutcomes = await _moderationApiClient.GetAllModeratorPageReviewOutcomes(request.ApplicationId, request.UserId);
            if (savedOutcomes is null || !savedOutcomes.Any())
            {
                viewmodel.IsReadyForModeratorConfirmation = false;
            }
            else
            {
                // TODO: Can this be part of ModeratorApplicationViewModel rather than injecting things outside?
                // Inject the statuses into viewmodel
                foreach (var sequence in viewmodel.Sequences)
                {
                    foreach (var section in sequence.Sections)
                    {
                        if (string.IsNullOrEmpty(section.Status))
                        {
                            if (sequence.SequenceNumber == SequenceIds.DeliveringApprenticeshipTraining && section.SectionNumber == SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees)
                            {
                                var sectorsChosen = await _moderationApiClient.GetModeratorSectors(request.ApplicationId, request.UserId);
                                section.Status = GetSectorsSectionStatus(sectorsChosen, savedOutcomes);
                            }
                            else
                            {
                                var sectionPageReviewOutcomes = savedOutcomes.Where(p =>
                                    p.SequenceNumber == sequence.SequenceNumber &&
                                    p.SectionNumber == section.SectionNumber).ToList();

                                section.Status = GetSectionStatus(sectionPageReviewOutcomes);
                            }
                        }
                    }
                }

                viewmodel.IsReadyForModeratorConfirmation = IsReadyForModeratorConfirmation(viewmodel);
            }

            return viewmodel;
        }

        public string GetSectionStatus(List<ModeratorPageReviewOutcome> sectionPageReviewOutcomes)
        {
            var sectionStatus = string.Empty;

            if (sectionPageReviewOutcomes != null && sectionPageReviewOutcomes.Any())
            {
                if (sectionPageReviewOutcomes.Count.Equals(1))
                {
                    // The section only has 1 question
                    sectionStatus = sectionPageReviewOutcomes[0].Status;
                }
                else
                {
                    // The section contains multiple question
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

        public string GetSectorsSectionStatus(IEnumerable<ModeratorSector> sectorsChosen, IEnumerable<ModeratorPageReviewOutcome> savedOutcomes)
        {
            var sectionPageReviewOutcomes = savedOutcomes?.Where(p =>
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

        private static bool IsReadyForModeratorConfirmation(ModeratorApplicationViewModel viewmodel)
        {
            var isReadyForModeratorConfirmation = true;

            foreach (var sequence in viewmodel.Sequences)
            {
                foreach (var section in sequence.Sections)
                {
                    if (string.IsNullOrEmpty(section.Status) || (!section.Status.Equals(ModeratorSectionStatus.Pass) &&
                                                   !section.Status.Equals(ModeratorSectionStatus.Fail) &&
                                                   !section.Status.Equals(ModeratorSectionStatus.NotRequired) &&
                                                   !section.Status.Contains(ModeratorSectionStatus.FailOutOf)))
                    {
                        isReadyForModeratorConfirmation = false;
                        break;
                    }
                }

                if (!isReadyForModeratorConfirmation) break;
            }

            return isReadyForModeratorConfirmation;
        }
    }
}
