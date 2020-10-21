using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class GetModeratorOutcomeRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }

        public GetModeratorOutcomeRequest(Guid applicationId, string userId)
        {
            ApplicationId = applicationId;
            UserId = userId;
        }
    }
}