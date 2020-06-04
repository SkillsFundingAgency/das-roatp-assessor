using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Consts;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class SupplementaryInformationService : ISupplementaryInformationService
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly ILogger<SupplementaryInformationService> _logger;

        public SupplementaryInformationService(ILogger<SupplementaryInformationService> logger, IRoatpApplicationApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;
            _logger = logger;
        }

        public async Task<List<AssessorSupplementaryInformation>> GetSupplementaryInformation(Guid applicationId, string pageId)
        {
            var supplementaryInformation = new List<AssessorSupplementaryInformation>();

            if (pageId == RoatpWorkflowPageIds.SafeguardingPolicyIncludesPreventDutyPolicy)
            {
                var safeGuardingPolicySupplementaryInformation = await GetSafeguardingPolicySupplementaryInformation(applicationId);

                if (safeGuardingPolicySupplementaryInformation != null)
                {
                    supplementaryInformation.Add(safeGuardingPolicySupplementaryInformation);
                }
            }

            return supplementaryInformation;
        }

        private async Task<AssessorSupplementaryInformation> GetSafeguardingPolicySupplementaryInformation(Guid applicationId)
        {
            const int safegaurdingPolicySequenceNumber = 4;
            const int safegaurdingPolicySectionNumber = 4;
            const string safegaurdingPolicyPageId = RoatpWorkflowPageIds.SafeguardingPolicy;
            const string safegaurdingPolicyLabel = "Safeguarding policy";

            AssessorSupplementaryInformation supplementaryInformation = null;

            var page = await _applyApiClient.GetAssessorPage(applicationId, safegaurdingPolicySequenceNumber, safegaurdingPolicySectionNumber, safegaurdingPolicyPageId);

            if (page?.Questions?.First() != null)
            {
                var questionId = page.Questions.First().QuestionId;

                supplementaryInformation = new AssessorSupplementaryInformation
                {
                    ApplicationId = page.ApplicationId,
                    SequenceNumber = page.SequenceNumber,
                    SectionNumber = page.SectionNumber,
                    PageId = page.PageId,
                    Question = page.Questions.First(q => q.QuestionId == questionId),
                    Answer = page.Answers.First(a => a.QuestionId == questionId)
                };

                supplementaryInformation.Question.Label = safegaurdingPolicyLabel;
            }

            return supplementaryInformation;
        }
    }
}
