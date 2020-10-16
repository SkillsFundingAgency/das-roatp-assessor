namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class InProgressApplicationsViewModel : AssessorDashboardViewModel
    {
        public InProgressApplicationsViewModel(string currentUser, int newApplications, int inProgressApplications, int moderationApplications, int clarificationApplications, int closedApplications)
            : base("InProgressApplications", newApplications, inProgressApplications, moderationApplications, clarificationApplications, closedApplications)
        {
            CurrentUser = currentUser;
        }

        public string CurrentUser { get; }
    }
}
