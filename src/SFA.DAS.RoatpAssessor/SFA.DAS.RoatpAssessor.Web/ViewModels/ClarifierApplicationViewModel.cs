using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using System;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ClarifierApplicationViewModel : OrganisationDetailsViewModel
    {
        public Guid Id { get; }
        public Guid ApplicationId { get; }
        public Guid OrgId { get; }

        public string ApplicationStatus { get; }
        public string ModerationStatus { get; set; }

        public string Assessor1Name { get; set; }
        public string Assessor2Name { get; set; }

        public string ModeratorName { get; set; }
        public DateTime? ClarificationRequestedDate { get; set; }

        public List<ClarificationSequence> Sequences { get; }
        public bool IsReadyForClarificationConfirmation { get; set; }

        public ClarifierApplicationViewModel(Apply application, Contact contact, List<ClarificationSequence> sequences, string userId)
        {
            Id = application.Id;
            ApplicationId = application.ApplicationId;
            OrgId = application.OrganisationId;

            ApplicantEmailAddress = contact.Email;

            ApplicationStatus = application.ApplicationStatus;
            ModerationStatus = application.ModerationStatus;

            Assessor1Name = application.Assessor1Name;
            Assessor2Name = application.Assessor2Name;

            if (application.ApplyData?.ApplyDetails != null)
            {
                ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
                Ukprn = application.ApplyData.ApplyDetails.UKPRN;
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName;
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
            }

            if (application.ApplyData?.ModeratorReviewDetails != null)
            {
                ClarificationRequestedDate = application.ApplyData.ModeratorReviewDetails.ClarificationRequestedOn;
                ModeratorName = application.ApplyData.ModeratorReviewDetails.ModeratorName;
            }

            Sequences = sequences;
        }

        public string GetStatusCss(string status)
        {
            switch (status)
            {
                case null:
                    return string.Empty;
                case string a when a.Equals(SectionStatus.Pass, StringComparison.InvariantCultureIgnoreCase):
                    return "app-task-list__tag das-tag das-tag--solid-green";
                case string b when b.Equals(SectionStatus.Fail, StringComparison.InvariantCultureIgnoreCase):
                    return "app-task-list__tag das-tag das-tag--solid-red";
                case string c when c.Equals(SectionStatus.Clarification, StringComparison.InvariantCultureIgnoreCase):
                case string d when d.Equals(SectionStatus.InProgress, StringComparison.InvariantCultureIgnoreCase):
                    return "app-task-list__tag das-tag";
                case string e when e.Equals(SectionStatus.NotRequired, StringComparison.InvariantCultureIgnoreCase):
                    return "app-task-list__tag das-tag das-tag--solid-grey";
                default:
                    return string.Empty;
            }
        }
    }
}
