using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Services;

namespace SFA.DAS.RoatpAssessor.Web.Controllers
{
    public class SectionReviewController : Controller
    {
        private readonly ISectionReviewOrchestrator _sectionReviewOrchestrator;
        public SectionReviewController(ISectionReviewOrchestrator sectionReviewOrchestrator)
        {
            _sectionReviewOrchestrator = sectionReviewOrchestrator;
        }

        [HttpGet]
        public async Task<IActionResult> ReviewAnswers(Guid applicationId, int sequenceNumber, int sectionNumber)
        {
            var username = User.UserDisplayName();

            var viewModel = await _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(applicationId, username, sequenceNumber, sectionNumber));

            if (viewModel is null)
            {
                return Redirect($"/Home/{applicationId}");
            }

            return View("~/Views/Home/ReviewAnswers.cshtml", viewModel);
        }
    }
}