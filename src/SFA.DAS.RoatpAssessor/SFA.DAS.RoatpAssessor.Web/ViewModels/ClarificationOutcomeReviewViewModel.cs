using System;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ClarificationOutcomeReviewViewModel : OrganisationDetailsViewModel
    {
        public Guid ApplicationId { get; set; }

        public string Status { get; set; }

        public string ReviewComment { get; set; }
        public string ConfirmStatus { get; set; }
    }
}