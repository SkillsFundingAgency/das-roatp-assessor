using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Outcome
{
    public class ClarificationOutcome
    {
        public Guid ApplicationId { get; set; }
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }

        public string ClarifierUserName { get; set; }
        public string ClarifierUserId { get; set; }
        public string ClarificationReviewStatus { get; set; }
        public string ClarificationReviewComment { get; set; }
        public string ClarificationResponse { get; set; }
        public List<string> ClarificationFiles { get; set; }
    }
}
