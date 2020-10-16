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
        Task<ApplicationCounts> GetApplicationCounts(string userId);

        Task<List<AssessorApplicationSummary>> GetNewApplications(string userId);
        Task<List<AssessorApplicationSummary>> GetInProgressApplications(string userId);
        Task<List<ModerationApplicationSummary>> GetInModerationApplications(string userId);
        Task<List<ClarificationApplicationSummary>> GetInClarificationApplications(string userId);
        Task<List<ClosedApplicationSummary>> GetClosedApplications(string userId);

        Task<Apply> GetApplication(Guid applicationId);

        Task<Contact> GetContactForApplication(Guid applicationId);

        Task<HttpResponseMessage> DownloadFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string questionId, string filename);
    }
}
