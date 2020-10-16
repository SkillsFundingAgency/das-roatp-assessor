using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public abstract class AssessorDashboardViewModel : DashboardViewModel
    {
        protected AssessorDashboardViewModel(string currentPage, int newApplications, int inProgressApplications, int moderationApplications, int clarificationApplications, int closedApplications)
            : base(currentPage, newApplications, inProgressApplications, moderationApplications, clarificationApplications, closedApplications)
        {
            Applications = new List<ApplicationViewModel>();
        }

        public List<ApplicationViewModel> Applications { get; }

        public void AddApplication(ApplicationViewModel application)
        {
            Applications.Add(application);
        }
    }
}
