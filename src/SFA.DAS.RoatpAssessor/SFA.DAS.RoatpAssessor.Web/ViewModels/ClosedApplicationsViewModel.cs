using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ClosedApplicationsViewModel : DashboardViewModel
    {
        public ClosedApplicationsViewModel(string currentUser, int newApplications, int inProgressApplications, int moderationApplications, int clarificationApplications, int closedApplications)
            : base("ClosedApplications", newApplications, inProgressApplications, moderationApplications, clarificationApplications, closedApplications)
        {
            CurrentUser = currentUser;
            Applications = new List<ClosedApplicationViewModel>();
        }

        public string CurrentUser { get; }

        public List<ClosedApplicationViewModel> Applications { get; }

        public void AddApplication(ClosedApplicationViewModel application)
        {
            Applications.Add(application);
        }
    }
}
