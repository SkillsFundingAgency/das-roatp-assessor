using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ModeratorSectorDetailsViewModel : SectorDetailsViewModel
    {
        public new ModeratorSectorDetails SectorDetails { get; set; }

        public BlindAssessmentOutcome BlindAssessmentOutcome { get; set; }
    }
}
