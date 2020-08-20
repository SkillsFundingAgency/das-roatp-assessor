using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class GetAssessorPageReviewOutcomeRequest
    {
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }
        public string UserId { get; set; }
    }
}
