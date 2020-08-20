using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class GetModeratorOverviewRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }

        public GetModeratorOverviewRequest(Guid applicationId, string userId)
        {
            ApplicationId = applicationId;
            UserId = userId;
        }
    }
}
