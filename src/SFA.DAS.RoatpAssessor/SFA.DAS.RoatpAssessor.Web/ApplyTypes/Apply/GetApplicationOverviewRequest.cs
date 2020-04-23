using System;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply
{
    public class GetApplicationOverviewRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }

        public GetApplicationOverviewRequest(Guid applicationId, string userName)
        {
            ApplicationId = applicationId;
            UserName = userName;
        }
    }
}
