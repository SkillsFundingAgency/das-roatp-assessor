using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
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

            var viewmodel = new AssessorApplicationViewModel(application);
            viewmodel.Sequences = await _applyApiClient.GetAssessorSequences(application.ApplicationId);

            var savedStatuses = await _applyApiClient.GetAssessorSectionAnswers(application.ApplicationId);
            if (savedStatuses is null || !savedStatuses.Any())
            {
                viewmodel.IsReadyForModeration = false;
            }
            else
            {
                // Inject the statuses into viewmodel
                foreach (var currentStatus in savedStatuses)
                {
                    var sequence = viewmodel.Sequences.FirstOrDefault(seq => seq.SequenceNumber == currentStatus.SequenceNumber);
                    var section = sequence?.Sections.FirstOrDefault(sec => sec.SectionNumber == currentStatus.SectionNumber);

                    if(section != null)
                    {
                        section.Status = currentStatus.Status;
                    }
                }

                viewmodel.IsReadyForModeration = IsReadyForModeration(viewmodel);
            }

            return viewmodel;
        }

        public bool IsReadyForModeration(AssessorApplicationViewModel viewmodel)
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
