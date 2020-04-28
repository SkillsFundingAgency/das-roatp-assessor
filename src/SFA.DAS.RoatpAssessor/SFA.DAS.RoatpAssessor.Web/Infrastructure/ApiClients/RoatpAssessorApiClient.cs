using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients
{
    public class RoatpAssessorApiClient : ApiClientBase<RoatpAssessorApiClient>, IRoatpAssessorApiClient
    {
        public RoatpAssessorApiClient(HttpClient httpClient, ILogger<RoatpAssessorApiClient> logger, IRoatpApplicationTokenService tokenService) : base(httpClient, logger)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenService.GetToken(_httpClient.BaseAddress));
        }

        public async Task<List<RoatpAssessorApplicationSummary>> GetNewApplications(string userId)
        {
            return await Get<List<RoatpAssessorApplicationSummary>>($"Assessor/Applications/{userId}/New");
        }
    }
}
