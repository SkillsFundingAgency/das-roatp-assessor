using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Consts;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using System;
using System.Linq;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ClarifierReviewAnswersViewModel : ReviewAnswersViewModel
    {
        public ModerationOutcome ModerationOutcome { get; set; }

        public bool ClarificationRequired => ModerationOutcome?.ModeratorReviewStatus != ModeratorPageReviewStatus.Pass;

        public bool ClarificationFileRequired
        {
            get
            {
                if (!ClarificationRequired) return false;

                var isFileUploadPage = Questions?.Any(q => "FileUpload".Equals(q.InputType, StringComparison.InvariantCultureIgnoreCase)) ?? false;
                var safeguardingPolicyIncludesPreventDutyPolicy = PageId == RoatpWorkflowPageIds.SafeguardingPolicyIncludesPreventDutyPolicy && SupplementaryInformation.Any(sp => sp.PageId == RoatpWorkflowPageIds.SafeguardingPolicy);

                return isFileUploadPage || safeguardingPolicyIncludesPreventDutyPolicy;
            }
        }
    }
}
