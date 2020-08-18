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
        Task<RoatpAssessorSummary> GetAssessorSummary(string userId); // Maybe call objects GetDashboardCounts

        Task<List<RoatpAssessorApplicationSummary>> GetNewApplications(string userId); // Maybe return a RoatpApplicationSummary object
        Task<List<RoatpAssessorApplicationSummary>> GetInProgressApplications(string userId); // Maybe return a RoatpApplicationSummary object
        Task<List<RoatpModerationApplicationSummary>> GetModerationApplications(string userId); // Maybe return a RoatpApplicationSummary object

        Task<Apply> GetApplication(Guid applicationId);

        Task<Contact> GetContactForApplication(Guid applicationId);

        Task<HttpResponseMessage> DownloadFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string questionId, string filename);
    }
}
