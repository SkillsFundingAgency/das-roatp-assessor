using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply
{
    public class GetReviewAnswersRequest
    {
        public Guid ApplicationId { get; }
        public string UserId { get; }
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }

        public GetReviewAnswersRequest(Guid applicationId, string userId, int sequenceNumber, int sectionNumber, string pageId)
        {
            ApplicationId = applicationId;
            UserId = userId;
            SequenceNumber = sequenceNumber;
            SectionNumber = sectionNumber;
            PageId = pageId;
        }
    }
}
