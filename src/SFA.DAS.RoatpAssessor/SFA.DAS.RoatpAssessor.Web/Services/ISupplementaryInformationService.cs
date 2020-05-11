using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface ISupplementaryInformationService
    {
        Task<List<AssessorSupplementaryInformation>> GetSupplementaryInformation(Guid applicationId, string pageId);
    }
}