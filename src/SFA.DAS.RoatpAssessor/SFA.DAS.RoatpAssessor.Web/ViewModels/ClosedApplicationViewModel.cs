using System;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ClosedApplicationViewModel : ApplicationViewModel
    {
        public string OutcomeMadeBy { get; set; }
        public string ModerationStatus { get; set; }
        public DateTime OutcomeMadeDate { get; set; }
    }
}
