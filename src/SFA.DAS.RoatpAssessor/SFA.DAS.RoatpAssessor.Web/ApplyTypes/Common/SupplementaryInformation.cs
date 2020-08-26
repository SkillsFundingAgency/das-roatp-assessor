using System;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common
{
    public class SupplementaryInformation
    {
        // Note this is needed when a page requires to pull information from different pages
        public Guid ApplicationId { get; set; }

        public int SequenceNumber { get; set; }

        public int SectionNumber { get; set; }

        public string PageId { get; set; }

        public Question Question { get; set; }

        public Answer Answer { get; set; }
    }
}
