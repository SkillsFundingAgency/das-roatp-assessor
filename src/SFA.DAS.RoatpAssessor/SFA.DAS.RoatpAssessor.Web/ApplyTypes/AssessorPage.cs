using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes
{
    public class AssessorPage
    {
        public Guid ApplicationId { get; set; }

        public int SequenceNumber { get; set; }

        public int SectionNumber { get; set; }

        public string PageId { get; set; }

        public string NextPageId { get; set; }

        public string SequenceTitle { get; set; }
        public string SectionTitle { get; set; }
        public string Title { get; set; }

        public string BodyText { get; set; }

        public List<AssessorQuestion> Questions { get; set; }

        public List<AssessorAnswer> Answers { get; set; }
    }
}
