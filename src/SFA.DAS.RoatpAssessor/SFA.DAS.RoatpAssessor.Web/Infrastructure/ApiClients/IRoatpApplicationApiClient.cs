using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Refit;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Outcome;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpApplicationApiClient
    {
        [Get("/Assessor/Applications/{userId}?searchTerm={searchTerm}")]
        Task<ApplicationCounts> GetApplicationCounts(string userId, string searchTerm);

        [Get("/Assessor/Applications/{userId}/New?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<AssessorApplicationSummary>> GetNewApplications(string userId, string searchTerm, string sortColumn, string sortOrder);

        [Get("/Assessor/Applications/{userId}/InProgress?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<AssessorApplicationSummary>> GetInProgressApplications(string userId, string searchTerm, string sortColumn,string sortOrder);

        [Get("/Assessor/Applications/{userId}/InModeration?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<ModerationApplicationSummary>> GetInModerationApplications(string userId, string searchTerm, string sortColumn, string sortOrder);

        [Get("/Assessor/Applications/{userId}/InClarification?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<ClarificationApplicationSummary>> GetInClarificationApplications(string userId, string searchTerm, string sortColumn, string sortOrder);

        [Get("/Assessor/Applications/{userId}/Closed?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<ClosedApplicationSummary>> GetClosedApplications(string userId, string searchTerm, string sortColumn, string sortOrder);

        [Get("/Application/{applicationId}")]
        Task<Apply> GetApplication(Guid applicationId);

        [Get("/Application/{applicationId}/Contact")]
        Task<Contact> GetContactForApplication(Guid applicationId);

        [Get("/Assessor/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/Questions/{questionId}/download/{filename}")]
        Task<HttpResponseMessage> DownloadFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string questionId, string filename);
    }
}
