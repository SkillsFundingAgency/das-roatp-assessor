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

            #region Sequences Stubbed Data
            viewmodel.Sequences = new List<AssessorSequence>
            {
                new AssessorSequence
                {
                    SequenceNumber = 4,
                    DisplaySequenceNumber = 1,
                    SequenceTitle = "Protecting your apprentices checks",
                    Sections = new List<AssessorSection>
                    {
                        new AssessorSection { SectionNumber = 2, DisplaySectionNumber = 1, LinkTitle = "Continuity plan for apprenticeship training", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 3, DisplaySectionNumber = 2, LinkTitle = "Equality and diversity policy", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 4, DisplaySectionNumber = 3, LinkTitle = "Safeguarding and Prevent duty policy", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 5, DisplaySectionNumber = 4, LinkTitle = "Health and safety policy", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 6, DisplaySectionNumber = 5, LinkTitle = "Acting as a subcontractor", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New }
                    }
                },

                new AssessorSequence
                {
                    SequenceNumber = 5,
                    DisplaySequenceNumber = 2,
                    SequenceTitle = "Readiness to engage checks",
                    Sections = new List<AssessorSection>
                    {
                        new AssessorSection { SectionNumber = 2, DisplaySectionNumber = 1, LinkTitle = "Engaging with employers", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 3, DisplaySectionNumber = 2, LinkTitle = "Complaints policy", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 4, DisplaySectionNumber = 3, LinkTitle = "Contract for services template with employers", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 5, DisplaySectionNumber = 4, LinkTitle = "Commitment statement template", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 6, DisplaySectionNumber = 5, LinkTitle = "Prior learning of apprentices", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 7, DisplaySectionNumber = 6, LinkTitle = "Working with subcontractors", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New }
                    }
                },

                new AssessorSequence
                {
                    SequenceNumber = 6,
                    DisplaySequenceNumber = 3,
                    SequenceTitle = "Planning apprenticeship training checks",
                    Sections = new List<AssessorSection>
                    {
                        new AssessorSection { SectionNumber = 2, DisplaySectionNumber = 1, LinkTitle = "Type of apprenticeship training", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 3, DisplaySectionNumber = 2, LinkTitle = "Supporting apprentices", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 4, DisplaySectionNumber = 3, LinkTitle = "Forecasting starts", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 5, DisplaySectionNumber = 4, LinkTitle = "Off the job training", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 6, DisplaySectionNumber = 5, LinkTitle = "Where apprentices will be trained", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New }
                    }
                },

                new AssessorSequence
                {
                    SequenceNumber = 7,
                    DisplaySequenceNumber = 4,
                    SequenceTitle = "Delivering apprenticeship training checks",
                    Sections = new List<AssessorSection>
                    {
                        new AssessorSection { SectionNumber = 2, DisplaySectionNumber = 1, LinkTitle = "Overall accountability for apprenticeships", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 3, DisplaySectionNumber = 2, LinkTitle = "Management hierarchy for apprenticeships", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 4, DisplaySectionNumber = 3, LinkTitle = "Quality and high standards in apprenticeship training", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 5, DisplaySectionNumber = 4, LinkTitle = "Developing and delivering training", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 6, DisplaySectionNumber = 5, LinkTitle = "Sectors and employee experience", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 7, DisplaySectionNumber = 6, LinkTitle = "Policy for professional development of employees", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New }
                    }
                },

                new AssessorSequence
                {
                    SequenceNumber = 8,
                    DisplaySequenceNumber = 5,
                    SequenceTitle = "Evaluating apprenticeship training checks",
                    Sections = new List<AssessorSection>
                    {
                        new AssessorSection { SectionNumber = 2, DisplaySectionNumber = 1, LinkTitle = "Process for evaluating the quality of training delivered", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 3, DisplaySectionNumber = 2, LinkTitle = "Evaluating the quality of apprenticeship training", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New },
                        new AssessorSection { SectionNumber = 4, DisplaySectionNumber = 3, LinkTitle = "Systems and processes to collect apprenticeship data", Status = SectionReviewStatus.New, StatusDescription = SectionReviewStatus.New }
                    }
                },
            };
            #endregion

            viewmodel.ReadyToConfirm = CheckIsItReadyToConfirm(viewmodel);

            return viewmodel;
        }

        public bool CheckIsItReadyToConfirm(AssessorApplicationViewModel viewmodel)
        {
            var isReadyToConfirm = true;

            foreach (var sequence in viewmodel.Sequences)
            {
                foreach (var section in sequence.Sections)
                {
                    if (section.Status == null || (!section.Status.Equals(SectionReviewStatus.Pass) && !section.Status.Equals(SectionReviewStatus.Fail) && !section.Status.Equals(SectionReviewStatus.NotRequired)))
                    {
                        isReadyToConfirm = false;
                        break;
                    }
                }
            }

            return isReadyToConfirm;
        }
    }
}
