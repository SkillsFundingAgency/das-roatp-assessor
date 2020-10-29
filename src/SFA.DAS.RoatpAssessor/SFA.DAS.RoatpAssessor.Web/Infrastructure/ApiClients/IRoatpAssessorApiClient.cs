using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpAssessorApiClient
    {
        Task<bool> AssignAssessor(Guid applicationId, AssignAssessorCommand request);

        Task<List<AssessorSequence>> GetAssessorSequences(Guid applicationId);

        Task<AssessorPage> GetAssessorPage(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId);

        Task<List<AssessorSector>> GetAssessorSectors(Guid applicationId, string userId);

        Task<AssessorSectorDetails> GetAssessorSectorDetails(Guid applicationId, string pageId);

        Task<bool> SubmitAssessorPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string userId, string userName, string status, string comment);

        Task<AssessorPageReviewOutcome> GetAssessorPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string userId);
        Task<List<AssessorPageReviewOutcome>> GetAssessorPageReviewOutcomesForSection(Guid applicationId, int sequenceNumber, int sectionNumber, string userId);
        Task<List<AssessorPageReviewOutcome>> GetAllAssessorPageReviewOutcomes(Guid applicationId, string userId);

        Task<bool> UpdateAssessorReviewStatus(Guid applicationId, string userId, string userName, string status);
    }
}
