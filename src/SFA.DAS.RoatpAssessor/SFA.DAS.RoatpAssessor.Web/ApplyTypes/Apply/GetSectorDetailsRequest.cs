using System;
using Microsoft.ApplicationInsights;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply
{
    public class GetSectorDetailsRequest
    {
        public Guid ApplicationId { get; }

        public string PageId { get; }
        public string UserId { get; }
        public GetSectorDetailsRequest(Guid applicationId, string pageId, string userId)
        {
            ApplicationId = applicationId;
            PageId = pageId;
            UserId = userId;
        }
    }
}