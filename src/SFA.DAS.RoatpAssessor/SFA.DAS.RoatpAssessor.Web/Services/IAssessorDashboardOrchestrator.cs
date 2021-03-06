﻿using System;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface IAssessorDashboardOrchestrator
    {
        Task<NewApplicationsViewModel> GetNewApplicationsViewModel(string userId, string searchTerm, string sortColumn, string sortOrder);

        Task<bool> AssignApplicationToAssessor(Guid applicationId, int assessorNumber, string assessorUserId, string assessorName);

        Task<InProgressApplicationsViewModel> GetInProgressApplicationsViewModel(string userId, string searchTerm, string sortColumn, string sortOrder);
    }
}
