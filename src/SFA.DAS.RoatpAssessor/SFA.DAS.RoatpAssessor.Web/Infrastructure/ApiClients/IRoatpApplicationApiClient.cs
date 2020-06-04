using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpApplicationApiClient
    {
        Task<Apply> GetApplication(Guid applicationId);

        Task<Contact> GetContactForApplication(Guid applicationId);

        Task<List<AssessorSequence>> GetAssessorSequences(Guid applicationId);

        Task<List<Sector>> GetChosenSectors(Guid applicationId);

        Task<AssessorPage> GetAssessorPage(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId);

        Task<bool> SubmitAssessorPageOutcome(Guid applicationId,
                                       int sequenceNumber,
                                       int sectionNumber,
                                       string pageId,
                                       int assessorType,
                                       string userId,
                                       string status,
                                       string comment);

        Task<PageReviewOutcome> GetPageReviewOutcome(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, int assessorType, string userId);
        Task<List<PageReviewOutcome>> GetAssessorReviewOutcomesPerSection(Guid applicationId, int sequenceNumber, int sectionNumber, int assessorType, string userId);
        Task<List<PageReviewOutcome>> GetAllAssessorReviewOutcomes(Guid applicationId, int assessorType, string userId);

        Task<HttpResponseMessage> DownloadFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string questionId, string filename);

        Task<bool> UpdateAssessorReviewStatus(Guid applicationId, int assessorType, string userId, string status);
    }
}
