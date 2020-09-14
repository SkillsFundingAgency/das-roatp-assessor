using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public abstract class SectorDetailsViewModel : ApplicationSummaryAndBreadcrumbViewModel
    {
        public string PageId { get; set; }
        public SectorDetails SectorDetails { get; set; }
    }
}
