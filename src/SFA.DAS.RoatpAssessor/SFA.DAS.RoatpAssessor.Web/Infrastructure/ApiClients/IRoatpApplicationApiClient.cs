using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpApplicationApiClient
    {
        Task<Apply> GetApplication(Guid applicationId);

        Task<List<dynamic>> GetAssessorSectionAnswers(Guid applicationId);
    }
}
