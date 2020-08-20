using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class SectorViewModel : ApplicationSummaryAndBreadcrumbViewModel
    {
        public string PageId { get; set; }
        public AssessorSectorDetails SectorDetails { get; set; }
    }
}
