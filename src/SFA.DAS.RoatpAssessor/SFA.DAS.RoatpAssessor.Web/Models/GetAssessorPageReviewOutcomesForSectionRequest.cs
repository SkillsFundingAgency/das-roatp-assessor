using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class GetAssessorPageReviewOutcomesForSectionRequest
    {
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public int AssessorType { get; set; }
        public string UserId { get; set; }
    }
}
