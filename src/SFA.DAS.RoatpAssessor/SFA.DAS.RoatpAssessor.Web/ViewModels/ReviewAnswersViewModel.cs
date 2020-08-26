using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ReviewAnswersViewModel : ApplicationSummaryAndBreadcrumbViewModel
    {      
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }

        public string NextPageId { get; set; }

        public List<Question> Questions { get; set; }
        public List<Answer> Answers { get; set; }
        public List<TabularData> TabularData { get; set; }
        public List<SupplementaryInformation> SupplementaryInformation { get; set; }

        public List<string> GuidanceInformation { get; set; }
    }
}
