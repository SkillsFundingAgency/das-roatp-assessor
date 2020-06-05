using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Helpers;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Domain;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class AssessorOverviewOrchestrator : IAssessorOverviewOrchestrator
    {
        private readonly ILogger<AssessorOverviewOrchestrator> _logger;
        private readonly IRoatpApplicationApiClient _applyApiClient;

        public AssessorOverviewOrchestrator(ILogger<AssessorOverviewOrchestrator> logger, IRoatpApplicationApiClient applyApiClient)
        {
            _logger = logger;
            _applyApiClient = applyApiClient;
        }

        public async Task<AssessorApplicationViewModel> GetOverviewViewModel(GetApplicationOverviewRequest request)
        {
            var application = await _applyApiClient.GetApplication(request.ApplicationId);
            var contact = await _applyApiClient.GetContactForApplication(request.ApplicationId);
            var sequences = await _applyApiClient.GetAssessorSequences(request.ApplicationId);

            if (application is null || contact is null || sequences is null)
            {
                return null;
            }

            var assessorType = AssessorReviewHelper.SetAssessorType(application, request.UserId);

            var viewmodel = new AssessorApplicationViewModel(application, contact, sequences, request.UserId);

            var savedOutcomes = await _applyApiClient.GetAllAssessorReviewOutcomes(request.ApplicationId, (int)assessorType, request.UserId);
            if (savedOutcomes is null || !savedOutcomes.Any())
            {
                viewmodel.IsReadyForModeration = false;
            }
            else
            {
                // TODO: Can this be part of AssessorApplicationViewModel rather than injecting things outside?
                // Inject the statuses into viewmodel
                foreach (var sequence in viewmodel.Sequences)
                {
                    foreach (var section in sequence.Sections)
                    {
                        if (string.IsNullOrEmpty(section.Status))
                        {
                            if (sequence.SequenceNumber == SequenceIds.DeliveringApprenticeshipTraining && section.SectionNumber == SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees)
                            {
                                section.Status = await GetSectionStatusForSectors(request, savedOutcomes);
                            }
                            else
                            {
                                var sectionPageReviewOutcomes = savedOutcomes.Where(p =>
                                    p.SequenceNumber == sequence.SequenceNumber &&
                                    p.SectionNumber == section.SectionNumber).ToList();
                                section.Status = GetSectionStatus(sectionPageReviewOutcomes, false);
                            }
                        }
                    }
                }

                viewmodel.IsReadyForModeration = IsReadyForModeration(viewmodel);
            }

            return viewmodel;
        }

        public string GetSectionStatus(List<PageReviewOutcome> sectionPageReviewOutcomes, bool sectorSection)
        {
            // TODO: It looks like this function belongs in AssessorApplicationViewModel and not to be publicly exposed in the AssessorOverviewOrchestrator
            // TODO: Convert into static function
            var sectionStatus = string.Empty;
            if(sectionPageReviewOutcomes != null && sectionPageReviewOutcomes.Any())
            {
                if (sectionPageReviewOutcomes.Count.Equals(1))
                {
                    sectionStatus = sectionPageReviewOutcomes[0].Status;
                }
                else
                {
                    var passStatusesCount = sectionPageReviewOutcomes.Count(p => p.Status == AssessorPageReviewStatus.Pass);
                    var failStatusesCount = sectionPageReviewOutcomes.Count(p => p.Status == AssessorPageReviewStatus.Fail);
                    var inProgressStatusesCount = sectionPageReviewOutcomes.Count(p => p.Status == AssessorPageReviewStatus.InProgress);
                    var noTagCount = sectionPageReviewOutcomes.Count(p => string.IsNullOrEmpty(p.Status));
                    {
                        sectionStatus = null;
                    }
                    else if (sectionPageReviewOutcomes.All(x => x.Status == AssessorPageReviewStatus.Pass))
                    {
                        sectionStatus = AssessorSectionStatus.Pass;
                    }
                    else if (sectionPageReviewOutcomes.All(p => p.Status == AssessorPageReviewStatus.Fail))
                    {
                        sectionStatus = AssessorSectionStatus.Fail;
                    }
                    else if (sectionPageReviewOutcomes.All(p => p.Status == AssessorPageReviewStatus.Pass || p.Status == AssessorPageReviewStatus.Fail))
                    {
                        sectionStatus = $"{sectionPageReviewOutcomes.Count(p => p.Status == AssessorPageReviewStatus.Fail)} {AssessorSectionStatus.FailOutOf} {sectionPageReviewOutcomes.Count}";
                    }
                        sectionStatus = sectorSection ? AssessorSectionStatus.Fail : $"{failStatusesCount} {AssessorSectionStatus.FailOutOf} {sectionPageReviewOutcomes.Count}";
                    else
                    {
                        sectionStatus = AssessorSectionStatus.InProgress;
                    }
                }
            }

            return sectionStatus;
        }


        private async Task<string> GetSectionStatusForSectors(GetApplicationOverviewRequest request, IEnumerable<PageReviewOutcome> savedOutcomes)
        {
            var sectorsChosen = await _applyApiClient.GetChosenSectors(request.ApplicationId, request.UserId);

            var sectorCount = sectorsChosen != null && sectorsChosen.Any() ? sectorsChosen.Count : 0;

            var sectionPageReviewOutcomes = savedOutcomes.Where(p =>
                p.SequenceNumber == SequenceIds.DeliveringApprenticeshipTraining &&
                p.SectionNumber == SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees).ToList();

            var sectorsWithValuesCount =
                sectionPageReviewOutcomes.Count(p => !string.IsNullOrEmpty(p.Status));

            if (sectorsWithValuesCount > 0 && sectorCount > 0 && sectorsWithValuesCount < sectorCount)
            {
                return AssessorSectionStatus.InProgress;
            }

            return GetSectionStatus(sectionPageReviewOutcomes, true);

        }
        private static bool IsReadyForModeration(AssessorApplicationViewModel viewmodel)
        {
            var isReadyForModeration = true;

            foreach (var sequence in viewmodel.Sequences)
            {
                foreach (var section in sequence.Sections)
                {
                    if (string.IsNullOrEmpty(section.Status) || (!section.Status.Equals(AssessorSectionStatus.Pass) && 
                                                   !section.Status.Equals(AssessorSectionStatus.Fail) && 
                                                   !section.Status.Equals(AssessorSectionStatus.NotRequired) &&
                                                   !section.Status.Contains(AssessorSectionStatus.FailOutOf)))
                    {
                        isReadyForModeration = false;
                        break;
                    }
                }

                if (!isReadyForModeration) break;
            }

            return isReadyForModeration;
        }
    }
}
