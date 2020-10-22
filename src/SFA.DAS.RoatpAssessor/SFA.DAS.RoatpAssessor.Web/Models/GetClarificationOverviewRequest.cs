using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class GetClarificationOverviewRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }

        public GetClarificationOverviewRequest(Guid applicationId, string userId)
        {
            ApplicationId = applicationId;
            UserId = userId;
        }
    }
}
