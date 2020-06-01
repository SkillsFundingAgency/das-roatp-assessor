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
using System.Threading.Tasks;

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
                            var sectionPageReviewOutcomes = savedOutcomes.Where(p => p.SequenceNumber == sequence.SequenceNumber && p.SectionNumber == section.SectionNumber).ToList();
                            section.Status = SetSectionStatus(sectionPageReviewOutcomes);
                        }
                    }
                }

                viewmodel.IsReadyForModeration = IsReadyForModeration(viewmodel);
            }

            return viewmodel;
        }

        public string SetSectionStatus(List<PageReviewOutcome> sectionPageReviewOutcomes)
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
                    if (sectionPageReviewOutcomes.All(p => string.IsNullOrEmpty(p.Status)))
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
                    else
                    {
                        sectionStatus = AssessorSectionStatus.InProgress;
                    }
                }
            }

            return sectionStatus;
        }

        private static bool IsReadyForModeration(AssessorApplicationViewModel viewmodel)
        {
            // TODO: It looks like this function belongs in AssessorApplicationViewModel and not to be publicly exposed in the AssessorOverviewOrchestrator
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
