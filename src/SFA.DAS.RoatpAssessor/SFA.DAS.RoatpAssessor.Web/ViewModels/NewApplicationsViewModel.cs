namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class NewApplicationsViewModel : AssessorDashboardViewModel
    {
        public NewApplicationsViewModel(int newApplications, int inProgressApplications, int moderationApplications, int clarificationApplications, int closedApplications)
            : base("NewApplications", newApplications, inProgressApplications, moderationApplications, clarificationApplications, closedApplications)
        {
        }
    }
}
