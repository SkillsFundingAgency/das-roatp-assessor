using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Domain;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpAssessorApiClient
    {
        Task<RoatpAssessorSummary> GetAssessorSummary(string userId);
        Task<List<RoatpAssessorApplicationSummary>> GetNewApplications(string userId);
        Task AssignAssessor(Guid applicationId, AssignAssessorApplicationRequest request);
    }
}
