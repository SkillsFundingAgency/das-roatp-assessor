using System;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ApplicationSummaryAndBreadcrumbViewModel
    {

        public Guid ApplicationId { get; set; }
        public string Heading { get; set; }
        public string Caption { get; set; }
        public string ApplyLegalName { get; set; }
        public string Ukprn { get; set; }
        public string ApplicationRoute { get; set; }
        public DateTime? SubmittedDate { get; set; }

        public string Status { get; set; }

        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }
        public string OptionInProgressText { get; set; }

        public string ApplicantEmailAddress { get; set; }

        public string ApplicationRouteShortText
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ApplicationRoute))
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
