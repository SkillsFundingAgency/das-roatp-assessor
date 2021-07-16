using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Outcome;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpApplicationApiClient
    {
        Task<ApplicationCounts> GetApplicationCounts(string userId, string searchTerm);

        Task<List<AssessorApplicationSummary>> GetNewApplications(string userId, string searchTerm, string sortColumn, string sortOrder);
        Task<List<AssessorApplicationSummary>> GetInProgressApplications(string userId, string searchTerm, string sortColumn,string sortOrder);
        Task<List<ModerationApplicationSummary>> GetInModerationApplications(string userId, string searchTerm, string sortColumn, string sortOrder);
        Task<List<ClarificationApplicationSummary>> GetInClarificationApplications(string userId, string searchTerm, string sortColumn, string sortOrder);
        Task<List<ClosedApplicationSummary>> GetClosedApplications(string userId, string searchTerm, string sortColumn, string sortOrder);

        Task<Apply> GetApplication(Guid applicationId);

        Task<Contact> GetContactForApplication(Guid applicationId);

        Task<HttpResponseMessage> DownloadFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string questionId, string filename);
    }
}
