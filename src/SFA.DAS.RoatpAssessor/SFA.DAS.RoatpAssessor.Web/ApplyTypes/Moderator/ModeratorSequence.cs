using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator
{
    public class ModeratorSequence
    {
        public int SequenceNumber { get; set; }
        public string SequenceTitle { get; set; }
        public List<ModeratorSection> Sections { get; set; }
    }
}
