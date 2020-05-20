using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class AssessorApplicationViewModel : OrganisationDetailsViewModel
    {
        public Guid Id { get; }
        public Guid ApplicationId { get; }
        public Guid OrgId { get; }
     
        public string ApplicationStatus { get; }
        public string AssessorReviewStatus { get; private set; }

        public bool IsAssessorApproved { get; set; }

        public List<AssessorSequence> Sequences { get; }
        public bool IsReadyForModeration { get; set; }

        public AssessorApplicationViewModel(Apply application, Contact contact, List<AssessorSequence> sequences, string userId)
        {
            Id = application.Id;
            ApplicationId = application.ApplicationId;
            OrgId = application.OrganisationId;

            ApplicantEmailAddress = contact.Email;

            ApplicationStatus = application.ApplicationStatus;
            SetAssessorReviewStatus(application, userId);

            if (application.ApplyData?.ApplyDetails != null)
            {
                ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
                Ukprn = application.ApplyData.ApplyDetails.UKPRN;
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName;
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
            }

            Sequences = sequences;
        }

        private void SetAssessorReviewStatus(Apply application, string userId)
        {
            if(application.Assessor1UserId == userId)
            {
                AssessorReviewStatus = application.Assessor1ReviewStatus;
                IsAssessorApproved = application.Assessor1ReviewStatus == ApplyTypes.AssessorReviewStatus.Approved;
            }
            else if(application.Assessor2UserId == userId)
            {
                AssessorReviewStatus = application.Assessor2ReviewStatus;
                IsAssessorApproved = application.Assessor2ReviewStatus == ApplyTypes.AssessorReviewStatus.Approved;
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
