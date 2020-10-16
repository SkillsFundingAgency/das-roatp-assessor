using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class InClarificationApplicationsViewModel : DashboardViewModel
    {
        public InClarificationApplicationsViewModel(string currentUser, int newApplications, int inProgressApplications, int moderationApplications, int clarificationApplications, int closedApplications)
            : base("InClarificationApplications", newApplications, inProgressApplications, moderationApplications, clarificationApplications, closedApplications)
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
