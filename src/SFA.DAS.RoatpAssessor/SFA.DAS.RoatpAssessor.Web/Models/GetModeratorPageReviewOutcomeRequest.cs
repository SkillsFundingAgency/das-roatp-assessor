namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class GetModeratorPageReviewOutcomeRequest
    {
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }
        public string UserId { get; set; }
    }
}
