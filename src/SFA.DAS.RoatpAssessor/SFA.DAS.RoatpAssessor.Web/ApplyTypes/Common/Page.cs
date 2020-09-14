using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common
{
    public class Page
    {
        public Guid ApplicationId { get; set; }

        public int SequenceNumber { get; set; }

        public int SectionNumber { get; set; }

        public string PageId { get; set; }

        public string NextPageId { get; set; }

        public string DisplayType { get; set; }

        public string Caption { get; set; }
        public string Heading { get; set; }

        public string Title { get; set; }
        public string BodyText { get; set; }

        public IEnumerable<Question> Questions { get; set; }

        public IEnumerable<Answer> Answers { get; set; }

        public IEnumerable<string> GuidanceInformation { get; set; }
    }
}
