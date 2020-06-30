using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Domain;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpModerationApiClient
    {
        Task<List<RoatpModerationApplicationSummary>> GetModerationApplications();
    }
}
