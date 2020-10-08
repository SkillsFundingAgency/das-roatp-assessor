using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using System;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Outcome
{
    public class ClosedApplicationSummary : ApplicationSummary
    {
        public string OutcomeStatus { get; set; }
        public DateTime OutcomeDate { get; set; }
    }
}
