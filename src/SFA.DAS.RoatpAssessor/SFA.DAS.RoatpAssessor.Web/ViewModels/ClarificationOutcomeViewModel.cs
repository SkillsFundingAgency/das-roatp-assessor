using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ClarificationOutcomeViewModel : OrganisationDetailsViewModel
    {
        public Guid Id { get; }
        public Guid ApplicationId { get; }
        public Guid OrgId { get; }
        public string ApplicationStatus { get; }
        public string ModerationStatus { get; set; }

        public int PassCount { get; set; }
        public int FailCount { get; set; }
        public string Status { get; set; }


        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }

        public ClarificationOutcomeViewModel(Guid applicationId)
        {
            ApplicationId = applicationId;
        }
        public ClarificationOutcomeViewModel(Apply application, List<ClarificationPageReviewOutcome> outcomes)
        {
            Id = application.Id;
            ApplicationId = application.ApplicationId;
            OrgId = application.OrganisationId;

            ApplicationStatus = application.ApplicationStatus;
            ModerationStatus = application.ModerationStatus;

            PassCount = outcomes?.Count(o => o.Status == ClarificationPageReviewStatus.Pass) ?? 0;
            FailCount = outcomes?.Count(o => o.Status == ClarificationPageReviewStatus.Fail) ?? 0;

            if (application.ApplyData?.ApplyDetails != null)
            {
                ApplicationReference = application.ApplyData.ApplyDetails.ReferenceNumber;
                ApplicationRoute = application.ApplyData.ApplyDetails.ProviderRouteName;
                Ukprn = application.ApplyData.ApplyDetails.UKPRN;
                ApplyLegalName = application.ApplyData.ApplyDetails.OrganisationName;
                SubmittedDate = application.ApplyData.ApplyDetails.ApplicationSubmittedOn;
            }
        }
    }
}