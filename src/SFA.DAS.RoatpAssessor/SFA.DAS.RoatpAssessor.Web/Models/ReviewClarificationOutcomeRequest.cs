using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class ReviewClarificationOutcomeRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }
        public string Status { get; }
        public string ReviewComment { get; }
        public ReviewClarificationOutcomeRequest(Guid applicationId, string userId, string status, string reviewComment)
        {
            ApplicationId = applicationId;
            UserId = userId;
            Status = status;
            ReviewComment = reviewComment;
        }
    }
}