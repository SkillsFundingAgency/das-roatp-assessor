using System;

namespace SFA.DAS.RoatpAssessor.Web.Domain
{
    public class RoatpModerationApplicationSummary : RoatpAssessorApplicationSummary
    {
        public ModerationStatus Status { get; set; }
    }
}
