using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public class RoatpApplicationApiClient : ApiClientBase<RoatpApplicationApiClient>, IRoatpApplicationApiClient
    {
        public RoatpApplicationApiClient(HttpClient httpClient, ILogger<RoatpApplicationApiClient> logger, IRoatpApplicationTokenService tokenService) : base(httpClient, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(_httpClient.BaseAddress));
        }

        public async Task<Apply> GetApplication(Guid applicationId)
        {
            return await Get<Apply>($"/Application/{applicationId}");
        }

        public async Task<List<dynamic>> GetAssessorSectionAnswers(Guid applicationId)
        {
            var answers = new List<dynamic>
            {
                new { SequenceNumber = 4, SectionNumber = 2, Status = "Fail", StatusDescription = "Fail" },
                new { SequenceNumber = 4, SectionNumber = 3, Status = "In progress", StatusDescription = "In progress" },
                new { SequenceNumber = 4, SectionNumber = 4, Status = "Fail", StatusDescription = "2 Fails out of 4" },
                new { SequenceNumber = 4, SectionNumber = 5, Status = "Pass", StatusDescription = "Pass" },
                new { SequenceNumber = 5, SectionNumber = 2, Status = "Not required", StatusDescription = "Not required" }
            };

            return await Task.FromResult(answers);
        }
    }
}
