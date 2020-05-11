using System;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes
{
    public class AssessorSupplementaryInformation
    {
        // Note this is needed when a page requires to pull information from different pages
        public Guid ApplicationId { get; set; }

        public int SequenceNumber { get; set; }

        public int SectionNumber { get; set; }

        public string PageId { get; set; }

        public AssessorQuestion Question { get; set; }

        public AssessorAnswer Answer { get; set; }
    }
}
