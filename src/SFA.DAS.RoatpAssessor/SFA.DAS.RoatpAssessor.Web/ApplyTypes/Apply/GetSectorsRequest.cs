using System;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply
{
    public class GetSectorsRequest
    {
        public Guid ApplicationId { get; }

        public string UserId { get; }

        public GetSectorsRequest(Guid applicationId, string userId)
        {
            ApplicationId = applicationId;
            UserId = userId;
        }
    }
}
