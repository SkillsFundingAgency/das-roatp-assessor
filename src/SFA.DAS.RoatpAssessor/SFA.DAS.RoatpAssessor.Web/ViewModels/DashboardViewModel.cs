﻿namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public abstract class DashboardViewModel
    {
        protected DashboardViewModel(string currentPage, int newApplications, int inProgressApplications, int moderationApplications, int clarificationApplications)
        {
            CurrentPage = currentPage;
            NewApplications = newApplications;
            InProgressApplications = inProgressApplications;
            ModerationApplications = moderationApplications;
            ClarificationApplications = clarificationApplications;
        }

        public string CurrentPage { get; }
        public int NewApplications { get; }
        public int InProgressApplications { get; }
        public int ModerationApplications { get; }
        public int ClarificationApplications { get; }
    }
}