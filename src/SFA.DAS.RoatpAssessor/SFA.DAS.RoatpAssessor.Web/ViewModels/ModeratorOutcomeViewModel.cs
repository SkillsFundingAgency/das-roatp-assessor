﻿using System;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ModeratorOutcomeViewModel : OrganisationDetailsViewModel
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
        public string OptionAskForClarificationText { get; set; }

        public ModeratorOutcomeViewModel(Apply application, string userId)
        {
            Id = application.Id;
            ApplicationId = application.ApplicationId;
            OrgId = application.OrganisationId;



            ApplicationStatus = application.ApplicationStatus;
            ModerationStatus = application.ModerationStatus;

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