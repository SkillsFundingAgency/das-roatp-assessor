using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
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

            var sequences = await _applyApiClient.GetAssessorSequences(application.ApplicationId);
            if (sequences is null)
            {
                return null;
            }

            //TODO: Can't access the user until staff idams is enabled
            //TODO: Can this be put in the request or determined in Apply Service? Less Assessor needs to know the better
            var assessorType = AssessorType.FirstAssessor; // SetAssessorType(application, request.UserId);

            var viewmodel = new AssessorApplicationViewModel(application, sequences, request.UserId);

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
                        sectionStatus = string.Format(AssessorSectionStatus.FailOutOf, failStatusesCount, sectionPageReviewOutcomes.Count);
                    }
                    else
                    {
                        sectionStatus = "Unhandled scenario"; // It should not happen. It's just for testing.
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
                    // TODO: Rework the logic according to requirements. Attention about AssessorSectionStatus.FailOutOf
                    if (section.Status == null || (!section.Status.Equals(AssessorSectionStatus.Pass) && 
                                                   !section.Status.Equals(AssessorSectionStatus.Fail) && 
                                                   !section.Status.Equals(AssessorSectionStatus.NotRequired) &&
                                                   !section.Status.Contains("OUT", StringComparison.InvariantCultureIgnoreCase)))
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
