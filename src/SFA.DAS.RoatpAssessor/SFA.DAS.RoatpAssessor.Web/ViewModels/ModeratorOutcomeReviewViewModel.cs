using System;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ModeratorOutcomeReviewViewModel
    {
        public string ApplicationRoute { get; set; }

        public string Ukprn { get; set; }
        public string ApplyLegalName { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public string Status { get; set; }

        public string ReviewComment { get; set; }
        public string ApplicantEmailAddress { get; set; }
    }
}