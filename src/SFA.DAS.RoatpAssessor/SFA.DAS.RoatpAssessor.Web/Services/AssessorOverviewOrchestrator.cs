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
            viewmodel.Sequences = GetCoreAssessorSequences();

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

        private List<AssessorSequence> GetCoreAssessorSequences()
        {
            return new List<AssessorSequence>
            {
                new AssessorSequence
                {
                    SequenceNumber = 4,
                    SequenceTitle = "Protecting your apprentices checks",
                    Sections = new List<AssessorSection>
                    {
                        new AssessorSection { SectionNumber = 2, LinkTitle = "Continuity plan for apprenticeship training", Status = "" },
                        new AssessorSection { SectionNumber = 3, LinkTitle = "Equality and diversity policy", Status = "" },
                        new AssessorSection { SectionNumber = 4, LinkTitle = "Safeguarding and Prevent duty policy", Status = "" },
                        new AssessorSection { SectionNumber = 5, LinkTitle = "Health and safety policy", Status = "" },
                        new AssessorSection { SectionNumber = 6, LinkTitle = "Acting as a subcontractor", Status = "" }
                    }
                },

                new AssessorSequence
                {
                    SequenceNumber = 5,
                    SequenceTitle = "Readiness to engage checks",
                    Sections = new List<AssessorSection>
                    {
                        new AssessorSection { SectionNumber = 2, LinkTitle = "Engaging with employers", Status = "" },
                        new AssessorSection { SectionNumber = 3, LinkTitle = "Complaints policy", Status = "" },
                        new AssessorSection { SectionNumber = 4, LinkTitle = "Contract for services template with employers", Status = "" },
                        new AssessorSection { SectionNumber = 5, LinkTitle = "Commitment statement template", Status = "" },
                        new AssessorSection { SectionNumber = 6, LinkTitle = "Prior learning of apprentices", Status = "" },
                        new AssessorSection { SectionNumber = 7, LinkTitle = "Working with subcontractors", Status = "" }
                    }
                },

                new AssessorSequence
                {
                    SequenceNumber = 6,
                    SequenceTitle = "Planning apprenticeship training checks",
                    Sections = new List<AssessorSection>
                    {
                        new AssessorSection { SectionNumber = 2, LinkTitle = "Type of apprenticeship training", Status = "" },
                        new AssessorSection { SectionNumber = 3, LinkTitle = "Supporting apprentices", Status = "" },
                        new AssessorSection { SectionNumber = 4, LinkTitle = "Forecasting starts", Status = "" },
                        new AssessorSection { SectionNumber = 5, LinkTitle = "Off the job training", Status = "" },
                        new AssessorSection { SectionNumber = 6, LinkTitle = "Where apprentices will be trained", Status = "" }
                    }
                },

                new AssessorSequence
                {
                    SequenceNumber = 7,
                    SequenceTitle = "Delivering apprenticeship training checks",
                    Sections = new List<AssessorSection>
                    {
                        new AssessorSection { SectionNumber = 2, LinkTitle = "Overall accountability for apprenticeships", Status = "" },
                        new AssessorSection { SectionNumber = 3, LinkTitle = "Management hierarchy for apprenticeships", Status = "" },
                        new AssessorSection { SectionNumber = 4, LinkTitle = "Quality and high standards in apprenticeship training", Status = "" },
                        new AssessorSection { SectionNumber = 5, LinkTitle = "Developing and delivering training", Status = "" },
                        new AssessorSection { SectionNumber = 6, LinkTitle = "Sectors and employee experience", Status = "" },
                        new AssessorSection { SectionNumber = 7, LinkTitle = "Policy for professional development of employees", Status = "" }
                    }
                },

                new AssessorSequence
                {
                    SequenceNumber = 8,
                    SequenceTitle = "Evaluating apprenticeship training checks",
                    Sections = new List<AssessorSection>
                    {
                        new AssessorSection { SectionNumber = 2, LinkTitle = "Process for evaluating the quality of training delivered", Status = "" },
                        new AssessorSection { SectionNumber = 3, LinkTitle = "Evaluating the quality of apprenticeship training", Status = "" },
                        new AssessorSection { SectionNumber = 4, LinkTitle = "Systems and processes to collect apprenticeship data", Status = "" }
                    }
                },
            };
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
