namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class UpdateAssessorReviewStatusCommand
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Status { get; set; }
    }
}
