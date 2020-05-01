using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply
{
    public class GetReviewAnswersRequest
    {
        public Guid ApplicationId { get; }
        public string UserName { get; }
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }

        public GetReviewAnswersRequest(Guid applicationId, string userName, int sequenceNumber, int sectionNumber, string pageId)
        {
            ApplicationId = applicationId;
            UserName = userName;
            SequenceNumber = sequenceNumber;
            SectionNumber = sectionNumber;
            PageId = pageId;
        }
    }
}
