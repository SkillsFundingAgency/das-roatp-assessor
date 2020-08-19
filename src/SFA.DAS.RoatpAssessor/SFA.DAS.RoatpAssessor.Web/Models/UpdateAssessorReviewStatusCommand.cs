namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class UpdateAssessorReviewStatusCommand
    {
        public int AssessorType { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
    }
}
