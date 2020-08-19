using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpAssessorApiClient
    {
        Task AssignAssessor(Guid applicationId, AssignAssessorApplicationRequest request);

        Task<List<AssessorSequence>> GetAssessorSequences(Guid applicationId);

        Task<AssessorPage> GetAssessorPage(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId);

        Task<List<Sector>> GetChosenSectors(Guid applicationId, string userId);

        Task<SectorDetails> GetSectorDetails(Guid applicationId, string pageId);

        Task<bool> SubmitAssessorPageOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, int assessorType, string userId, string status, string comment);

        Task<PageReviewOutcome> GetPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, int assessorType, string userId);
        Task<List<PageReviewOutcome>> GetAssessorReviewOutcomesPerSection(Guid applicationId, int sequenceNumber, int sectionNumber, int assessorType, string userId);
        Task<List<PageReviewOutcome>> GetAllAssessorReviewOutcomes(Guid applicationId, int assessorType, string userId);

        Task<bool> UpdateAssessorReviewStatus(Guid applicationId, int assessorType, string userId, string status);
    }
}
