using System;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ApplicationViewModel
    {
        public string OrganisationName { get; }
        public string Ukprn { get; }
        public string ApplicationReferenceNumber { get; }
        public string ProviderRoute { get; }
        public DateTime SubmittedDate { get; }
        public string Assessor1 { get; }
        public string Assessor2 { get; }
    }
}
