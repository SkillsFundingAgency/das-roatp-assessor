using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ReviewAnswersViewModel
    {
        public Guid ApplicationId { get; set; }

        // Will not need them. Just for testing
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
    }
}
