using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public abstract class ReviewAnswersViewModel : ApplicationReviewViewModel
    {      
        public List<Question> Questions { get; set; }
        public List<Answer> Answers { get; set; }
        public List<TabularData> TabularData { get; set; }
        public List<SupplementaryInformation> SupplementaryInformation { get; set; }

        public List<string> GuidanceInformation { get; set; }
    }
}
