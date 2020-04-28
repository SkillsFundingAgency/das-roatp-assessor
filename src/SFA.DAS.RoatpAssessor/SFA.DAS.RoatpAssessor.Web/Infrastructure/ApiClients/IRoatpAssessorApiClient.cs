using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Domain;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpAssessorApiClient
    {
        Task<List<RoatpAssessorApplicationSummary>> GetNewApplications(string userId);
    }
}
