using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using RestEase;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Outcome;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpApplicationApiClient
    {
        [Get("Assessor/Applications/{userId}?searchTerm={searchTerm}")]
        Task<ApplicationCounts> GetApplicationCounts([Path] string userId, [Path] string searchTerm);

        [Get("Assessor/Applications/{userId}/New?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<AssessorApplicationSummary>> GetNewApplications([Path] string userId, [Path] string searchTerm, [Path] string sortColumn, [Path] string sortOrder);

        [Get("Assessor/Applications/{userId}/InProgress?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<AssessorApplicationSummary>> GetInProgressApplications([Path] string userId, [Path] string searchTerm, [Path] string sortColumn, [Path] string sortOrder);

        [Get("Assessor/Applications/{userId}/InModeration?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<ModerationApplicationSummary>> GetInModerationApplications([Path] string userId, [Path] string searchTerm, [Path] string sortColumn, [Path] string sortOrder);

        [Get("Assessor/Applications/{userId}/InClarification?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<ClarificationApplicationSummary>> GetInClarificationApplications([Path] string userId, [Path] string searchTerm, [Path] string sortColumn, [Path] string sortOrder);

        [Get("Assessor/Applications/{userId}/Closed?searchTerm={searchTerm}&sortColumn={sortColumn}&sortOrder={sortOrder}")]
        Task<List<ClosedApplicationSummary>> GetClosedApplications([Path] string userId, [Path] string searchTerm, [Path] string sortColumn, [Path] string sortOrder);

        [Get("/Application/{applicationId}")]
        Task<Apply> GetApplication([Path] Guid applicationId);

        [Get("/Application/{applicationId}/Contact")]
        Task<Contact> GetContactForApplication([Path] Guid applicationId);

        [Get("/Assessor/Applications/{applicationId}/Sequences/{sequenceNumber}/Sections/{sectionNumber}/Page/{pageId}/Questions/{questionId}/download/{filename}")]
        Task<HttpResponseMessage> DownloadFile([Path] Guid applicationId, [Path] int sequenceNumber, [Path] int sectionNumber, [Path] string pageId, [Path] string questionId, [Path] string filename);
    }
}
