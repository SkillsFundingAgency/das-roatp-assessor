using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class InProgressApplicationsViewModel : DashboardViewModel<ApplicationViewModel>
    {
        public InProgressApplicationsViewModel(string currentUser, int newApplications, int inProgressApplications, int moderationApplications, int clarificationApplications) : base("InProgressApplications", newApplications, inProgressApplications, moderationApplications, clarificationApplications)
        {
            CurrentUser = currentUser;
        }

        public string CurrentUser { get; }
    }
}
