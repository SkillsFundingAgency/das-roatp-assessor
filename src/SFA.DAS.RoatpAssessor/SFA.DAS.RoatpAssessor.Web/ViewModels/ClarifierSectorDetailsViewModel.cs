using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ClarifierSectorDetailsViewModel : SectorDetailsViewModel
    {
        public new ClarificationSectorDetails SectorDetails { get; set; }

        public ModerationOutcome ModerationOutcome { get; set; }

        public bool ClarificationRequired => ModerationOutcome?.ModeratorReviewStatus != ModeratorPageReviewStatus.Pass;
    }
}
