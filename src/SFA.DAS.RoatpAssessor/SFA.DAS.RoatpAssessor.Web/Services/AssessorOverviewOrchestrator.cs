﻿using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
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

            // TODO: To be implemented 
            var userId = "4dsfdg-MyGuidUserId-yf6re";
            var assessorType = AssessorType.FirstAssessor; // SetAssessorType(application, userId);

            var viewmodel = new AssessorApplicationViewModel(application, userId);
            viewmodel.Sequences = await _applyApiClient.GetAssessorSequences(application.ApplicationId);     

            var savedOutcomes = await _applyApiClient.GetAllAssessorReviewOutcomes(request.ApplicationId, (int)assessorType, userId);
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
                        var sectionPageReviewOutcomes = savedOutcomes.Where(p => p.SequenceNumber == sequence.SequenceNumber && p.SectionNumber == section.SectionNumber).ToList();
                        section.Status = SetSectionStatus(sectionPageReviewOutcomes);
                    }
                }

                viewmodel.IsReadyForModeration = IsReadyForModeration(viewmodel);
            }

            return viewmodel;
        }

        public string SetSectionStatus(List<PageReviewOutcome> sectionPageReviewOutcomes)
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
                    var passStatusesCount = sectionPageReviewOutcomes.Where(p => p.Status == AssessorPageReviewStatus.Pass).Count();
                    var failStatusesCount = sectionPageReviewOutcomes.Where(p => p.Status == AssessorPageReviewStatus.Fail).Count();
                    var inProgressStatusesCount = sectionPageReviewOutcomes.Where(p => p.Status == AssessorPageReviewStatus.InProgress).Count();
                    var noTagCount = sectionPageReviewOutcomes.Where(p => p.Status == null || p.Status == string.Empty).Count();
                    var allPassOrFail = sectionPageReviewOutcomes.Count.Equals(passStatusesCount + failStatusesCount);

                    if (sectionPageReviewOutcomes.Count.Equals(passStatusesCount))
                    {
                        sectionStatus = AssessorSectionStatus.Pass;
                    }
                    else if (sectionPageReviewOutcomes.Count.Equals(failStatusesCount))
                    {
                        sectionStatus = AssessorSectionStatus.Fail;
                    }
                    else if (noTagCount.Equals(0) && passStatusesCount.Equals(0) && failStatusesCount.Equals(0) && inProgressStatusesCount.Equals(0))
                    {
                        sectionStatus = string.Empty;
                    }
                    else if (inProgressStatusesCount > 0)
                    {
                        sectionStatus = AssessorSectionStatus.InProgress;
                    }
                    else if (inProgressStatusesCount.Equals(0) && !allPassOrFail)
                    {
                        sectionStatus = AssessorSectionStatus.InProgress;
                    }
                    else if (noTagCount.Equals(0) && inProgressStatusesCount.Equals(0) && allPassOrFail)
                    {
                        sectionStatus = string.Format(AssessorSectionStatus.FailOutOf, failStatusesCount, sectionPageReviewOutcomes.Count);
                    }
                    else
                    {
                        sectionStatus = "Unhandled scenario";
                    }
                }
            }

            return sectionStatus;
        }

        private static bool IsReadyForModeration(AssessorApplicationViewModel viewmodel)
        {
            var isReadyForModeration = true;

            foreach (var sequence in viewmodel.Sequences)
            {
                foreach (var section in sequence.Sections)
                {
                    if (section.Status == null || (!section.Status.Equals(SectionReviewStatus.Pass) && !section.Status.Equals(SectionReviewStatus.Fail) && !section.Status.Equals(SectionReviewStatus.NotRequired)))
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
