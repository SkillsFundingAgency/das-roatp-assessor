using System;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply
{
    public class GetReviewAnswersRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }
        public string NextPageId { get; set; }

        public GetReviewAnswersRequest(Guid applicationId, string userId, int sequenceNumber, int sectionNumber, string pageId, string nextPageId)
        {
            ApplicationId = applicationId;
            UserId = userId;
            SequenceNumber = sequenceNumber;
            SectionNumber = sectionNumber;
            PageId = pageId;
            NextPageId = nextPageId;
        }
    }
}
