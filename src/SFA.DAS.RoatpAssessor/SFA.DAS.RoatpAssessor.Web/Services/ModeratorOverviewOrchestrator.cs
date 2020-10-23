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
                // POTENTIAL TECH DEBT: Decide if processing of sequences should be contained within ModeratorApplicationViewModel rather than modifying this from outside.
                // This would result in better encapsulation of the logic but may cause issues if we need to inspect other sources
                foreach (var sequence in viewmodel.Sequences)
                {
                    foreach (var section in sequence.Sections)
                    {
                        if (string.IsNullOrEmpty(section.Status))
                        {
                            if (sequence.SequenceNumber == SequenceIds.DeliveringApprenticeshipTraining && section.SectionNumber == SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees)
                            {
                                section.Status = OverviewStatusService.GetSectorsSectionStatus(savedOutcomes);
                            }
                            else
                            {
                                section.Status = OverviewStatusService.GetSectionStatus(savedOutcomes, sequence.SequenceNumber, section.SectionNumber);
                            }
                        }
                    }
                }

                viewmodel.IsReadyForModeratorConfirmation = IsReadyForModeratorConfirmation(viewmodel);
            }

            return viewmodel;
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
                                                   !section.Status.Contains(ModeratorSectionStatus.FailOutOf) &&
                                                   !section.Status.Contains(ModeratorSectionStatus.FailsOutOf)))
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
