namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public abstract class ApplicationReviewViewModel : ApplicationSummaryAndBreadcrumbViewModel
    {      
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }

        public string NextPageId { get; set; }

        public string Heading { get; set; }
        public string Caption { get; set; }


        // TODO: Future Tech Debt - split the regions below into appropriate base classes.
        // The reason they are here is because different VMs inherit from it and this base class used a as a common type in several views
        // Note that it also affects ModelState & validation on the views

        #region Clarified Answer
        public string ClarificationResponse { get; set; }
        public string ClarificationFile { get; set; }
        #endregion

        #region Approval Section
        public string Status { get; set; }

        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }
        public string OptionInProgressText { get; set; }
        #endregion
    }
}
