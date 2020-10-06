using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class ReviewModeratorOutcomeRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }
        public string Status { get; }
        public string ReviewComment { get; }
        public ReviewModeratorOutcomeRequest(Guid applicationId, string userId, string status, string reviewComment)
        {
            ApplicationId = applicationId;
            UserId = userId;
            Status = status;
            ReviewComment = reviewComment;
        }
    }
}