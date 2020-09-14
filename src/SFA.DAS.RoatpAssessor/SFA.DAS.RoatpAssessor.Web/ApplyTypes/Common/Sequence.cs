using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common
{
    public class Sequence
    {
        public int SequenceNumber { get; set; }
        public string SequenceTitle { get; set; }
        public IEnumerable<Section> Sections { get; set; }
    }
}
