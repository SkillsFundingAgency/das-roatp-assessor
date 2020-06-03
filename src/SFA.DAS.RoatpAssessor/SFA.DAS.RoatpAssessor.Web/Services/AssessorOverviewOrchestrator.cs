﻿using Microsoft.Extensions.Logging;
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
            if (application is null)
            {
                return null;
            }

            var contact = await _applyApiClient.GetContactForApplication(application.ApplicationId);
            if (contact is null)
            {
                return null;
            }

            var sequences = await _applyApiClient.GetAssessorSequences(application.ApplicationId);
            if (sequences is null)
            {
                return null;
            }

            var assessorType = AssessorReviewHelpers.SetAssessorType(application, request.UserId);

            var viewmodel = new AssessorApplicationViewModel(application, contact, sequences, request.UserId);

            var savedOutcomes = await _applyApiClient.GetAllAssessorReviewOutcomes(request.ApplicationId, (int)assessorType, request.UserId);
            if (savedOutcomes is null || !savedOutcomes.Any())
            {
                viewmodel.IsReadyForModeration = false;
            }
            else
            {
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
                    var allPassOrFail = sectionPageReviewOutcomes.Count.Equals(passStatusesCount + failStatusesCount);

                    if (sectionPageReviewOutcomes.Count.Equals(noTagCount)) // All empty
                    {
                        sectionStatus = null;
                    }
                    else if (sectionPageReviewOutcomes.Count.Equals(passStatusesCount)) // All Pass
                    {
                        sectionStatus = AssessorSectionStatus.Pass;
                    }
                    else if (sectionPageReviewOutcomes.Count.Equals(failStatusesCount)) // All Fail
                    {
                        sectionStatus = AssessorSectionStatus.Fail;
                    }
                    else if (inProgressStatusesCount > 0) // One or more 'In Progress'
                    {
                        sectionStatus = AssessorSectionStatus.InProgress;
                    }
                    else if ((!passStatusesCount.Equals(0) && !allPassOrFail) || (!failStatusesCount.Equals(0) && !allPassOrFail)) // One or more Pass or Fail, but NOT all
                    {
                        sectionStatus = AssessorSectionStatus.InProgress;
                    }
                    else if (noTagCount.Equals(0) && inProgressStatusesCount.Equals(0) && allPassOrFail) // Not empty or 'In Progress', All either Pass or Fail
                    {
                        sectionStatus = sectorSection ? AssessorSectionStatus.Fail : $"{failStatusesCount} {AssessorSectionStatus.FailOutOf} {sectionPageReviewOutcomes.Count}";
                    }
                    else
                    {
                        sectionStatus = AssessorSectionStatus.Unknown; // It should not happen. It's just for testing.
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
                    // TODO: Rework the logic according to requirements. Attention about AssessorSectionStatus.FailOutOf
                    if (section.Status == null || (!section.Status.Equals(AssessorSectionStatus.Pass) && 
                                                   !section.Status.Equals(AssessorSectionStatus.Fail) && 
                                                   !section.Status.Equals(AssessorSectionStatus.NotRequired) &&
                                                   !section.Status.Contains(AssessorSectionStatus.FailOutOf)))
                    {
                        isReadyForModeration = false;
                        break;
                    }
                }
            }

            return isReadyForModeration;
        }
    }
}
