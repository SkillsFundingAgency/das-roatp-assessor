namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class SubmitClarificationPageReviewOutcomeCommand
    {
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }
        public string UserId { get; set; }
        public string ClarificationResponse { get; set; }
        public string Status { get; set; }
        public string Comment { get; set; }
    }
}
