using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class GetClarificationOutcomeRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }

        public GetClarificationOutcomeRequest(Guid applicationId, string userId)
        {
            ApplicationId = applicationId;
            UserId = userId;
        }
    }
}