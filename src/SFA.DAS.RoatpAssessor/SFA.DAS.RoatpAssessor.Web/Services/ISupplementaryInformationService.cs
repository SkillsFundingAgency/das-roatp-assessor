using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface ISupplementaryInformationService
    {
        Task<List<SupplementaryInformation>> GetSupplementaryInformation(Guid applicationId, string pageId);
    }
}