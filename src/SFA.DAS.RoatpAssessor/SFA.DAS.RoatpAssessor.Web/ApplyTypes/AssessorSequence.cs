using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes
{
    public class AssessorSequence
    {
        public int SequenceNumber { get; set; }
        public string SequenceTitle { get; set; }
        public List<AssessorSection> Sections { get; set; }
    }
}
