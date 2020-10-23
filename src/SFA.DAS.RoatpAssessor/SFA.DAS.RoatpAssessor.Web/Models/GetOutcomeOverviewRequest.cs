using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class GetOutcomeOverviewRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }

        public GetOutcomeOverviewRequest(Guid applicationId, string userId)
        {
            ApplicationId = applicationId;
            UserId = userId;
        }
    }
}