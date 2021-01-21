using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Consts;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class SupplementaryInformationService : ISupplementaryInformationService
    {
        private readonly IRoatpAssessorApiClient _assessorApiClient;

        public SupplementaryInformationService(IRoatpAssessorApiClient assessorApiClient)
        {
            _assessorApiClient = assessorApiClient;
        }

        public async Task<List<SupplementaryInformation>> GetSupplementaryInformation(Guid applicationId, string pageId)
        {
            var supplementaryInformation = new List<SupplementaryInformation>();

            if (pageId == RoatpWorkflowPageIds.SafeguardingPolicyIncludesPreventDutyPolicy)
            {
                const int safegaurdingPolicySequenceNumber = RoatpWorkflowSequenceIds.ProtectingYourApprentices;
                const int safegaurdingPolicySectionNumber = 4;

                var page = await _assessorApiClient.GetAssessorPage(applicationId, safegaurdingPolicySequenceNumber, safegaurdingPolicySectionNumber, pageId);
                var answer = page?.Answers.First().Value;

                // Only retrieve if it was included in the safeguarding policy
                if ("Yes".Equals(answer, StringComparison.InvariantCultureIgnoreCase))
                {
                    var safeGuardingPolicySupplementaryInformation = await GetSafeguardingPolicySupplementaryInformation(applicationId);

                    if (safeGuardingPolicySupplementaryInformation != null)
                    {
                        supplementaryInformation.Add(safeGuardingPolicySupplementaryInformation);
                    }
                }
            }

            return supplementaryInformation;
        }

        private async Task<SupplementaryInformation> GetSafeguardingPolicySupplementaryInformation(Guid applicationId)
        {
            const int safegaurdingPolicySequenceNumber = RoatpWorkflowSequenceIds.ProtectingYourApprentices;
            const int safegaurdingPolicySectionNumber = 4;
            const string safegaurdingPolicyPageId = RoatpWorkflowPageIds.SafeguardingPolicy;
            const string safegaurdingPolicyLabel = "Safeguarding policy";

            SupplementaryInformation supplementaryInformation = null;

            var page = await _assessorApiClient.GetAssessorPage(applicationId, safegaurdingPolicySequenceNumber, safegaurdingPolicySectionNumber, safegaurdingPolicyPageId);

            if (page?.Questions?.First() != null)
            {
                var questionId = page.Questions.First().QuestionId;

                supplementaryInformation = new SupplementaryInformation
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
