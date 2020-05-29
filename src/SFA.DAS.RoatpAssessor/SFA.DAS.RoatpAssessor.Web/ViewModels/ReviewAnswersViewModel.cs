using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ReviewAnswersViewModel
    {
        public Guid ApplicationId { get; set; }

        public string Status { get; set; }

        public string Ukprn { get; set; }
        public string ApplyLegalName { get; set; }
        public string ApplicationRoute { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string ApplicantEmailAddress { get; set; }

        public string Heading { get; set; }
        public string Caption { get; set; }

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

        public string ApplicationRouteShortText
        {
            get
            {
                if (String.IsNullOrWhiteSpace(ApplicationRoute))
                {
                    return string.Empty;
                }
                var index = ApplicationRoute.IndexOf(' ');
                if (index < 0)
                {
                    return ApplicationRoute;
                }
                return ApplicationRoute.Substring(0, index + 1);
            }
        }
    }
}
