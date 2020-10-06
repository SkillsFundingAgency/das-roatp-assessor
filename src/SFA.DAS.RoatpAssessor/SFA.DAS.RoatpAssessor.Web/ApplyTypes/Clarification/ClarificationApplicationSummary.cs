using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using System;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification
{
    public class ClarificationApplicationSummary : ApplicationSummary
    {
        public string ModeratorName { get; set; }
        public DateTime ClarificationRequestedDate { get; set; }
    }
}
