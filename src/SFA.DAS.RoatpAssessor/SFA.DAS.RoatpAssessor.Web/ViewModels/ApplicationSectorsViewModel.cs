using System.Collections.Generic;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ApplicationSectorsViewModel : ApplicationSummaryAndBreadcrumbViewModel
    {
        public IEnumerable<Sector> SelectedSectors { get; set; }
    }
}
