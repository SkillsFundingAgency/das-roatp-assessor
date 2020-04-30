using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public class RoatpApplicationApiClient : ApiClientBase<RoatpApplicationApiClient>, IRoatpApplicationApiClient
    {
        public RoatpApplicationApiClient(HttpClient httpClient, ILogger<RoatpApplicationApiClient> logger, IRoatpApplicationTokenService tokenService) : base(httpClient, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(_httpClient.BaseAddress));
        }

        public async Task<Apply> GetApplication(Guid applicationId)
        {
            return await Get<Apply>($"/Application/{applicationId}");
        }

        public async Task<List<AssessorSequence>> GetAssessorSequences(Guid applicationId)
        {
            var assessorSequences = await Get<List<AssessorSequence>>($"/Assessor/{applicationId}/Overview");

            // NOTE: TO BE REMOVED once we are happy with integrating with RoATP Apply API
            if (assessorSequences is null)
            {
                assessorSequences = new List<AssessorSequence>
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

            return assessorSequences;
        }

        public async Task<List<dynamic>> GetAssessorSectionAnswers(Guid applicationId)
        {
            // TODO: Needs completing once we know how we're going to store and retrieve answers
            var answers = new List<dynamic>
            {
                new { SequenceNumber = 4, SectionNumber = 2, Status = "Fail" },
                new { SequenceNumber = 4, SectionNumber = 3, Status = "In progress" },
                new { SequenceNumber = 4, SectionNumber = 4, Status = "2 Fails out of 4" },
                new { SequenceNumber = 4, SectionNumber = 5, Status = "Pass" },
                new { SequenceNumber = 5, SectionNumber = 2, Status = "Not required" }
            };

            return await Task.FromResult(answers);
        }
    }
}
