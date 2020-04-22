using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class NewApplicationsViewModel : DashboardViewModel
    {
        public NewApplicationsViewModel(int newApplications, int inProgressApplications, int moderationApplications, int clarificationApplications) : base("NewApplications", newApplications, inProgressApplications, moderationApplications, clarificationApplications)
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
