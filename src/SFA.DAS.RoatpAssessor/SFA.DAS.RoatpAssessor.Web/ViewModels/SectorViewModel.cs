using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class SectorViewModel : ApplicationSummaryAndBreadcrumbViewModel
    {
        public string PageId { get; set; }
        public AssessorType AssessorType { get; set; }
        public SectorDetails SectorDetails { get; set; }
    }
}
