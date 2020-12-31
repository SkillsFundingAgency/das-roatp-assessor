using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using System;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Outcome
{
    public class ClosedApplicationSummary : ApplicationSummary
    {
        public string ModerationStatus { get; set; }

        public DateTime OutcomeMadeDate { get; set; }
        public string OutcomeMadeBy { get; set; }  
    }
}
