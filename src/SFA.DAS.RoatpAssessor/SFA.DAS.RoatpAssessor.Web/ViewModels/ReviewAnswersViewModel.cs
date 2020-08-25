using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ReviewAnswersViewModel : ApplicationSummaryAndBreadcrumbViewModel
    {      
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }

        public string NextPageId { get; set; }

        public List<AssessorQuestion> Questions { get; set; }
        public List<AssessorAnswer> Answers { get; set; }
        public List<TabularData> TabularData { get; set; }
        public List<AssessorSupplementaryInformation> SupplementaryInformation { get; set; }

        public List<string> GuidanceInformation { get; set; }
    }
}
