using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class OutcomeOverviewOrchestrator : IOutcomeOverviewOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applicationApiClient;
        private readonly IRoatpClarificationApiClient _clarificationApiClient;

        public OutcomeOverviewOrchestrator(IRoatpApplicationApiClient applicationApiClient, IRoatpClarificationApiClient clarificationApiClient)
        {
            _applicationApiClient = applicationApiClient;
            _clarificationApiClient = clarificationApiClient;
        }

        public async Task<OutcomeApplicationViewModel> GetOverviewViewModel(GetOutcomeOverviewRequest request)
        {
            var application = await _applicationApiClient.GetApplication(request.ApplicationId);
            var contact = await _applicationApiClient.GetContactForApplication(request.ApplicationId);
            var sequences = await _clarificationApiClient.GetClarificationSequences(request.ApplicationId);

            if (application is null || contact is null || sequences is null)
            {
                return null;
            }

            var viewmodel = new OutcomeApplicationViewModel(application, contact, sequences);

            var savedOutcomes = await _clarificationApiClient.GetAllClarificationPageReviewOutcomes(request.ApplicationId, request.UserId);
            if (!(savedOutcomes is null) && savedOutcomes.Any())
            {
                foreach (var sequence in viewmodel.Sequences)
                {
                    foreach (var section in sequence.Sections)
                    {
                        if (string.IsNullOrEmpty(section.Status))
                        {
                            section.Status = OverviewStatusService.GetOutcomeSectionStatus(savedOutcomes, sequence.SequenceNumber, section.SectionNumber);
                        }
                    }
                }
            }

            return viewmodel;
        }
    }
}
