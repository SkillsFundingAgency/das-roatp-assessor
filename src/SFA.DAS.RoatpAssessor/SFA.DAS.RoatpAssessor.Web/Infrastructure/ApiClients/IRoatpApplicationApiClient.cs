using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public interface IRoatpApplicationApiClient
    {
        Task<Apply> GetApplication(Guid applicationId);

        Task<List<AssessorSequence>> GetAssessorSequences(Guid applicationId);

        Task<List<dynamic>> GetAssessorSectionAnswers(Guid applicationId);

        Task<AssessorPage> GetAssessorPage(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId);

        Task SubmitAssessorPageOutcome(Guid applicationId,
                                       int sequenceNumber,
                                       int sectionNumber,
                                       string pageId,
                                       int assessorType,
                                       string userId,
                                       string status,
                                       string comment);

        Task<HttpResponseMessage> DownloadFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string questionId, string filename);
    }
}
