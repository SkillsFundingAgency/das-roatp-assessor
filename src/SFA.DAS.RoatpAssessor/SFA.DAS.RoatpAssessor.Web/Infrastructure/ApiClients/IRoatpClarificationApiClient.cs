using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        Task<bool> SubmitClarificationPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string userId, string clarificationResponse, string status, string comment);

        Task<List<ClarificationPageReviewOutcome>> GetAllClarificationPageReviewOutcomes(Guid applicationId, string userId);
        Task<ClarificationPageReviewOutcome> GetClarificationPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string userId);
        Task<List<ClarificationPageReviewOutcome>> GetClarificationPageReviewOutcomesForSection(Guid applicationId, int sequenceNumber, int sectionNumber, string userId);
    }
}
