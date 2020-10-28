using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpClarificationApiClient
    {
        Task<List<ClarificationSequence>> GetClarificationSequences(Guid applicationId);

        Task<ClarificationPage> GetClarificationPage(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId);

        Task<List<ClarificationSector>> GetClarificationSectors(Guid applicationId, string userId);

        Task<ClarificationSectorDetails> GetClarificationSectorDetails(Guid applicationId, string pageId);

        Task<ModerationOutcome> GetModerationOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId);

        Task<bool> SubmitClarificationPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string userId, string clarificationResponse, string status, string comment, IFormFileCollection clarificationFiles);

        Task<List<ClarificationPageReviewOutcome>> GetAllClarificationPageReviewOutcomes(Guid applicationId, string userId);
        Task<ClarificationPageReviewOutcome> GetClarificationPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string userId);
        Task<List<ClarificationPageReviewOutcome>> GetClarificationPageReviewOutcomesForSection(Guid applicationId, int sequenceNumber, int sectionNumber, string userId);


        Task<HttpResponseMessage> DownloadFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string fileName);
        Task<HttpResponseMessage> DeleteFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string fileName);
    }
}
