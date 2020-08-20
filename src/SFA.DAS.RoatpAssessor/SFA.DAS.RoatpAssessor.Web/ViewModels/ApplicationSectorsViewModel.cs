using System.Collections.Generic;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ApplicationSectorsViewModel : ApplicationSummaryAndBreadcrumbViewModel
    {
        public List<AssessorSector> SelectedSectors { get; set; }
    }
}
