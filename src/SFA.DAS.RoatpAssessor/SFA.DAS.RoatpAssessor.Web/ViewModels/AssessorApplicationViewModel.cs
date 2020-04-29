using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class AssessorApplicationViewModel : OrganisationDetailsViewModel
    {
        public Guid Id { get; }
        public Guid ApplicationId { get; set; }
        public Guid OrgId { get; }

        public string ApplicationStatus { get; }
        public string AssessorReviewStatus { get; set; }

        public bool IsAssessorApproved { get; set; }

        public List<AssessorSequence> Sequences { get; set; }
        public bool IsReadyForModeration { get; set; }

        public AssessorApplicationViewModel(Apply application)
        {
            Id = application.Id;
            ApplicationId = application.ApplicationId;
            OrgId = application.OrganisationId;

            ApplicationStatus = application.ApplicationStatus;
            AssessorReviewStatus = application.AssessorReviewStatus;

            if (application.AssessorReviewStatus == ApplyTypes.AssessorReviewStatus.Approved)
            {
                IsAssessorApproved = true;
            }
            else if (application.AssessorReviewStatus == ApplyTypes.AssessorReviewStatus.Declined)
            {
                IsAssessorApproved = false;
            }

            if (application.ApplyData?.ApplyDetails != null)
            {
                ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
                Ukprn = application.ApplyData.ApplyDetails.UKPRN;
                OrganisationName = application.ApplyData.ApplyDetails.OrganisationName;
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
            }
        }

        public string GetStatusCss(string status)
        {
            switch (status)
            {
                case null: 
                    return string.Empty;
                case string a when a.Equals("pass", StringComparison.InvariantCultureIgnoreCase): 
                    return "app-task-list__task-pass";
                case string b when b.Contains("fail", StringComparison.InvariantCultureIgnoreCase): 
                    return "app-task-list__task-fail";
                case string c when c.Equals("in progress", StringComparison.InvariantCultureIgnoreCase):
                    return "app-task-list__task-inprogress";
                case string d when d.Equals("not required", StringComparison.InvariantCultureIgnoreCase):
                    return "app-task-list__task-inactive";
                default: 
                    return string.Empty;
            }
        }
    }
}
