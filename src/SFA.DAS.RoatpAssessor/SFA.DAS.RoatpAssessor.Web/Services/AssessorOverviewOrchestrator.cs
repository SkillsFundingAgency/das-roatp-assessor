using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;


namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class AssessorOverviewOrchestrator : IAssessorOverviewOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applicationApiClient;
        private readonly IRoatpAssessorApiClient _assessorApiClient;

        public AssessorOverviewOrchestrator(IRoatpApplicationApiClient applyApiClient, IRoatpAssessorApiClient assessorApiClient)
        {
            _applicationApiClient = applyApiClient;
            _assessorApiClient = assessorApiClient;
        }

        public async Task<AssessorApplicationViewModel> GetOverviewViewModel(GetAssessorOverviewRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var contact = await _applicationApiClient.GetContactForApplication(request.ApplicationId);
            var sequences = await _assessorApiClient.GetAssessorSequences(request.ApplicationId);

            if (application is null || contact is null || sequences is null)
            {
                return null;
            }

            var viewmodel = new AssessorApplicationViewModel(application, contact, sequences, request.UserId);

            var savedOutcomes = await _assessorApiClient.GetAllAssessorPageReviewOutcomes(request.ApplicationId, request.UserId);
            if (savedOutcomes is null || !savedOutcomes.Any())
            {
                viewmodel.IsReadyForModeration = false;
            }
            else
            {
                // POTENTIAL TECH DEBT: Decide if processing of sequences should be contained within AssessorApplicationViewModel rather than modifying this from outside.
                // This would result in better encapsulation of the logic but may cause issues if we need to inspect other sources
                foreach (var sequence in viewmodel.Sequences)
                {
                    foreach (var section in sequence.Sections)
                    {
                        if (string.IsNullOrEmpty(section.Status))
                        {
                            section.Status = OverviewStatusService.GetAssessorSectionStatus(savedOutcomes, sequence.SequenceNumber, section.SectionNumber);
                        }
                    }
                }

                viewmodel.IsReadyForModeration = IsReadyForModeration(viewmodel);
            }

            return viewmodel;
        }

        private static bool IsReadyForModeration(AssessorApplicationViewModel viewmodel)
        {
            var isReadyForModeration = true;

            foreach (var sequence in viewmodel.Sequences)
            {
                foreach (var section in sequence.Sections)
                {
                    if (string.IsNullOrEmpty(section.Status) || (!section.Status.Equals(SectionStatus.Pass) &&
                                                   !section.Status.Equals(SectionStatus.Fail) &&
                                                   !section.Status.Equals(SectionStatus.NotRequired) &&
                                                   !section.Status.Contains(SectionStatus.FailOutOf) &&
                                                   !section.Status.Contains(SectionStatus.FailsOutOf)))
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
