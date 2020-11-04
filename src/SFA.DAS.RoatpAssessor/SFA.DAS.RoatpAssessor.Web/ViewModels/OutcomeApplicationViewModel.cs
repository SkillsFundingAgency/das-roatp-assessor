using System;
using System.Collections.Generic;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class OutcomeApplicationViewModel : OrganisationDetailsViewModel
    {
        public Guid Id { get; }
        public Guid ApplicationId { get; }
        public Guid OrgId { get; }

        public string ApplicationStatus { get; }
        public string ModerationStatus { get; set; }

        public string Assessor1Name { get; set; }
        public string Assessor2Name { get; set; }

        public List<ClarificationSequence> Sequences { get; }
        public bool IsReadyForModeratorConfirmation { get; set; }

        public string ModeratorName { get; set; }
        public DateTime? OutcomeDate { get; set; }
        public string Outcome { get; set; }
        public string OutcomeComments { get; set; }

        public OutcomeApplicationViewModel(Apply application, Contact contact, List<ClarificationSequence> sequences,
            string userId)
        {
            Id = application.Id;
            ApplicationId = application.ApplicationId;
            OrgId = application.OrganisationId;

            ApplicantEmailAddress = contact.Email;

            ApplicationStatus = application.ApplicationStatus;
            ModerationStatus = application.ModerationStatus;

            Assessor1Name = application.Assessor1Name;
            Assessor2Name = application.Assessor2Name;
            Outcome = application.ModerationStatus;

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
                var moderatorDetails = application.ApplyData.ModeratorReviewDetails;
                ModeratorName = moderatorDetails.ModeratorName;
                OutcomeDate = moderatorDetails.OutcomeDateTime;
                OutcomeComments = moderatorDetails.ModeratorComments;
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
                case string b when b.Contains(SectionStatus.Fail, StringComparison.InvariantCultureIgnoreCase):
                    return "app-task-list__tag das-tag das-tag--solid-red";
                case string c when c.Equals(SectionStatus.InProgress, StringComparison.InvariantCultureIgnoreCase):
                    return "app-task-list__tag das-tag";
                case string d when d.Equals(SectionStatus.NotRequired, StringComparison.InvariantCultureIgnoreCase):
                    return "app-task-list__tag das-tag das-tag--solid-grey";
                default:
                    return string.Empty;
            }
        }
    }
}