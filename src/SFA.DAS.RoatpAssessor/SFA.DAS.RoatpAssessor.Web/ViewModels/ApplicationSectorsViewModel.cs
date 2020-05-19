using System;
using System.Collections.Generic;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ApplicationSectorsViewModel : ApplicationSummaryAndBreadcrumbViewModel
    {
        public List<Sector> SelectedSectors { get; set; }
    }
}
