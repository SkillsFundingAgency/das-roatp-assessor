using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public abstract class DashboardViewModel<T>
    {
        protected DashboardViewModel(string currentPage, int newApplications, int inProgressApplications, int moderationApplications, int clarificationApplications)
        {
            CurrentPage = currentPage;
            NewApplications = newApplications;
            InProgressApplications = inProgressApplications;
            ModerationApplications = moderationApplications;
            ClarificationApplications = clarificationApplications;
            Applications = new List<T>();
        }

        public string CurrentPage { get; }
        public int NewApplications { get; }
        public int InProgressApplications { get; }
        public int ModerationApplications { get; }
        public int ClarificationApplications { get; }
        public List<T> Applications { get; }

        public void AddApplication(T application)
        {
            Applications.Add(application);
        }
    }
}
