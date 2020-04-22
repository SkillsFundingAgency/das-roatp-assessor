using System;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ApplicationViewModel
    {
        public string OrganisationName { get; set; }
        public string Ukprn { get; set; }
        public string ApplicationReferenceNumber { get; set; }
        public string ProviderRoute { get; set; }
        public DateTime SubmittedDate { get; set; }
        public string Assessor1 { get; set; } 
        public string Assessor2 { get; set; }
    }
}
