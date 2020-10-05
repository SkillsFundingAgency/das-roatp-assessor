using System;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ClarificationApplicationViewModel : ApplicationViewModel
    {
        public string ClarificationStatus { get; set; }
        public string ModeratorName { get; set; }
        public DateTime ClarificationRequestedDate { get; set; }
    }
}
