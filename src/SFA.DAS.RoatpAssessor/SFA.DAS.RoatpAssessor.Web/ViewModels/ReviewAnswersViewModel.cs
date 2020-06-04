using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ReviewAnswersViewModel: ApplicationSummaryAndBreadcrumbViewModel
    {
  
        public string Status { get; set; }
        public string ApplicantEmailAddress { get; set; }

        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }
        public string OptionInProgressText { get; set; }

        // It seems that we will need them for grouping statuses
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }

        public string NextPageId { get; set; }
        public AssessorType AssessorType { get; set; }

        public List<AssessorQuestion> Questions { get; set; }
        public List<AssessorAnswer> Answers { get; set; }
        public List<TabularData> TabularData { get; set; }
        public List<AssessorSupplementaryInformation> SupplementaryInformation { get; set; }

        public List<string> GuidanceInformation { get; set; }
    }
}
