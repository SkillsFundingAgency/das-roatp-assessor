using System;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply
{
    public class GetApplicationOverviewRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }

        public GetApplicationOverviewRequest(Guid applicationId, string userId)
        {
            ApplicationId = applicationId;
            UserId = userId;
        }
    }
}
