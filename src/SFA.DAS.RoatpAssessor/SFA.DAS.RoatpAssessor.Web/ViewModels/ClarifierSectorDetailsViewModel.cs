using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ClarifierSectorDetailsViewModel : SectorDetailsViewModel
    {
        public new ClarificationSectorDetails SectorDetails { get; set; }

        public ModerationOutcome ModerationOutcome { get; set; }
    }
}
