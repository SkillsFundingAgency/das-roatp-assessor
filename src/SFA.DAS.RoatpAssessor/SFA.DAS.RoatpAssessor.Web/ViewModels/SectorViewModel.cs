using SFA.DAS.RoatpAssessor.Web.ApplyTypes;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class SectorViewModel : ApplicationSummaryAndBreadcrumbViewModel
    {
        public string PageId { get; set; }
        public SectorDetails SectorDetails { get; set; }
    }
}
