using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpApplicationApiClient
    {
        Task<ApplicationCounts> GetApplicationCounts(string userId);

        Task<List<RoatpAssessorApplicationSummary>> GetNewApplications(string userId);
        Task<List<RoatpAssessorApplicationSummary>> GetInProgressApplications(string userId);
        Task<List<RoatpModerationApplicationSummary>> GetModerationApplications(string userId);

        Task<Apply> GetApplication(Guid applicationId);

        Task<Contact> GetContactForApplication(Guid applicationId);

        Task<HttpResponseMessage> DownloadFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string questionId, string filename);
    }
}
