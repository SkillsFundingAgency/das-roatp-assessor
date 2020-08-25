using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class GetAssessorOverviewRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }

        public GetAssessorOverviewRequest(Guid applicationId, string userId)
        {
            ApplicationId = applicationId;
            UserId = userId;
        }
    }
}
