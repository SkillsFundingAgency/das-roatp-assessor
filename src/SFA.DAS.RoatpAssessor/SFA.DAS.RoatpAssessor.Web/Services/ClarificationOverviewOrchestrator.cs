using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class ClarificationOverviewOrchestrator : IClarificationOverviewOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applicationApiClient;
        private readonly IRoatpClarificationApiClient _clarificationApiClient;

        public ClarificationOverviewOrchestrator(IRoatpApplicationApiClient applicationApiClient, IRoatpClarificationApiClient clarificationApiClient)
        {
            _applicationApiClient = applicationApiClient;
            _clarificationApiClient = clarificationApiClient;
        }

        public async Task<ClarifierApplicationViewModel> GetOverviewViewModel(GetClarificationOverviewRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var contact = await _applicationApiClient.GetContactForApplication(request.ApplicationId);
            var sequences = await _clarificationApiClient.GetClarificationSequences(request.ApplicationId);

            if (application is null || contact is null || sequences is null)
            {
                return null;
            }

            var viewmodel = new ClarifierApplicationViewModel(application, contact, sequences, request.UserId);

            var savedOutcomes = await _clarificationApiClient.GetAllClarificationPageReviewOutcomes(request.ApplicationId, request.UserId);
            if (savedOutcomes is null || !savedOutcomes.Any())
            {
                viewmodel.IsReadyForClarificationConfirmation = false;
            }
            else
            {
                // POTENTIAL TECH DEBT: Decide if processing of sequences should be contained within ClarifierApplicationViewModel rather than modifying this from outside.
                // This would result in better encapsulation of the logic but may cause issues if we need to inspect other sources
                foreach (var sequence in viewmodel.Sequences)
                {
                    foreach (var section in sequence.Sections)
                    {
                        if (string.IsNullOrEmpty(section.Status))
                        {
                            section.Status = OverviewStatusService.GetClarificationSectionStatus(savedOutcomes, sequence.SequenceNumber, section.SectionNumber);
                        }
                    }
                }

                viewmodel.IsReadyForClarificationConfirmation = IsReadyForClarificationConfirmation(viewmodel);
            }

            return viewmodel;
        }

        private static bool IsReadyForClarificationConfirmation(ClarifierApplicationViewModel viewmodel)
        {
            var isReadyForClarificationConfirmation = true;

            foreach (var sequence in viewmodel.Sequences)
            {
                foreach (var section in sequence.Sections)
                {
                    if (string.IsNullOrEmpty(section.Status) || (!section.Status.Equals(SectionStatus.Pass) &&
                                                   !section.Status.Equals(SectionStatus.Fail) &&
                                                   !section.Status.Equals(SectionStatus.NotRequired)))
                    {
                        isReadyForClarificationConfirmation = false;
                        break;
                    }
                }

                if (!isReadyForClarificationConfirmation) break;
            }

            return isReadyForClarificationConfirmation;
        }
    }
}
