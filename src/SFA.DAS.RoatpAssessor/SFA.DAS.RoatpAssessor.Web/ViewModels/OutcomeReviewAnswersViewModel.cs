using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Outcome;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class OutcomeReviewAnswersViewModel : ReviewAnswersViewModel
    {
        public BlindAssessmentOutcome BlindAssessmentOutcome { get; set; }
        public ModerationOutcome ModerationOutcome { get; set; }
        public ClarificationOutcome ClarificationOutcome { get; set; }

        public bool ClarificationRequired => ModerationOutcome?.ModeratorReviewStatus != ModeratorPageReviewStatus.Pass;
    }
}
