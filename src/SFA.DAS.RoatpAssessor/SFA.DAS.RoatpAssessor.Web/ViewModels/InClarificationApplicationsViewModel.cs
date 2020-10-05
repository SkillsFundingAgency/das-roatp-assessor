using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class InClarificationApplicationsViewModel : DashboardViewModel
    {
        public InClarificationApplicationsViewModel(string currentUser, int newApplications, int inProgressApplications, int moderationApplications, int clarificationApplications) : base("InClarificationApplications", newApplications, inProgressApplications, moderationApplications, clarificationApplications)
        {
            CurrentUser = currentUser;
            Applications = new List<ClarificationApplicationViewModel>();
        }

        public string CurrentUser { get; }

        public List<ClarificationApplicationViewModel> Applications { get; }

        public void AddApplication(ClarificationApplicationViewModel application)
        {
            Applications.Add(application);
        }
    }
}
