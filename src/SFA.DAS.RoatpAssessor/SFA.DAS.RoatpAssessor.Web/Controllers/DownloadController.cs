using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class DownloadController : Controller
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;

        public DownloadController(IRoatpApplicationApiClient applyApiClient)
        {
            _applyApiClient = applyApiClient;
        }

        [HttpGet("Download/Application/{applicationId}/Sequence/{sequenceNo}/Section/{sectionNo}/Page/{pageId}/Question/{questionId}/Download/{filename}")]
        public async Task<IActionResult> DownloadFile(Guid applicationId, int sequenceNo, int sectionNo, string pageId, string questionId, string filename)
        {
            var response = await _applyApiClient.DownloadFile(applicationId, sequenceNo, sectionNo, pageId, questionId, filename);

            if (response.IsSuccessStatusCode)
            {
                var fileStream = await response.Content.ReadAsStreamAsync();

                return File(fileStream, response.Content.Headers.ContentType.MediaType, filename);
            }

            return NotFound();
        }
    }
}
