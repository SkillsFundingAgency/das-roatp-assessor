using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class InModerationApplicationsViewModel : DashboardViewModel
    {
        public InModerationApplicationsViewModel(string currentUser, int newApplications, int inProgressApplications, int moderationApplications, int clarificationApplications, int closedApplications)
            : base("InModerationApplications", newApplications, inProgressApplications, moderationApplications, clarificationApplications, closedApplications)
        {
            CurrentUser = currentUser;
            Applications = new List<ModerationApplicationViewModel>();
        }

        public string CurrentUser { get; }

        public List<ModerationApplicationViewModel> Applications { get; }

        public void AddApplication(ModerationApplicationViewModel application)
        {
            Applications.Add(application);
        }
    }
}
