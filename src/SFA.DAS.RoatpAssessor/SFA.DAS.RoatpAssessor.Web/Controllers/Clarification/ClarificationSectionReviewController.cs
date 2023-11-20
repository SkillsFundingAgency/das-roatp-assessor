using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.Extensions;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Controllers.Clarification
{
    [Authorize(Roles = Roles.RoatpAssessorTeam)]
    public class ClarificationSectionReviewController : ClarificationControllerBase<ClarificationSectionReviewController>
    {
        private readonly IClarificationSectionReviewOrchestrator _sectionReviewOrchestrator;
        public ClarificationSectionReviewController(IRoatpClarificationApiClient clarificationApiClient,
                                       IClarificationPageValidator clarificationPageValidator,
                                       IClarificationSectionReviewOrchestrator sectionReviewOrchestrator,
                                       ILogger<ClarificationSectionReviewController> logger) : base(clarificationApiClient, logger, clarificationPageValidator)
        {
            _sectionReviewOrchestrator = sectionReviewOrchestrator;
        }

        [HttpGet("ClarificationSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}")]
        [HttpGet("ClarificationSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}/Page/{pageId}")]
        public async Task<IActionResult> ReviewPageAnswers(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId)
        {
            var userId = HttpContext.User.UserId();


            if (sequenceNumber == SequenceIds.DeliveringApprenticeshipTraining && sectionNumber == SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees)
            {
                var sectorViewModel = await _sectionReviewOrchestrator.GetSectorsViewModel(new GetSectorsRequest(applicationId, userId));

                if (sectorViewModel is null)
                {
                    return RedirectToAction("ViewApplication", "ClarificationOverview", new { applicationId });
                }

                return View("~/Views/ClarificationSectionReview/ReviewSectors.cshtml", sectorViewModel);
            }

            var viewModel = await _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(applicationId, userId, sequenceNumber, sectionNumber, pageId, null));

            if (viewModel is null)
            {
                return RedirectToAction("ViewApplication", "ClarificationOverview", new { applicationId });
            }

            return View("~/Views/ClarificationSectionReview/ReviewAnswers.cshtml", viewModel);
        }

        [HttpPost("ClarificationSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}")]
        [HttpPost("ClarificationSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}/Page/{pageId}")]
        public async Task<IActionResult> ReviewPageAnswers(SubmitClarificationPageAnswerCommand command)
        {
            if(command.ClarificationRequired)
            {
                var userId = HttpContext.User.UserId();
                command.FilesToUpload = HttpContext.Request.Form.Files;

                Func<Task<ClarifierReviewAnswersViewModel>> viewModelBuilder = () => _sectionReviewOrchestrator.GetReviewAnswersViewModel(new GetReviewAnswersRequest(command.ApplicationId, userId, command.SequenceNumber, command.SectionNumber, command.PageId, command.NextPageId));

                return await ValidateAndUpdatePageAnswer(command, viewModelBuilder, $"~/Views/ClarificationSectionReview/ReviewAnswers.cshtml");
            }
            else if(string.IsNullOrEmpty(command.NextPageId))
            {
                return RedirectToAction("ViewApplication", "ClarificationOverview", new { applicationId = command.ApplicationId }, $"sequence-{command.SequenceNumber}");
            }
            else
            {
                return RedirectToAction("ReviewPageAnswers", "ClarificationSectionReview", new { applicationId = command.ApplicationId, sequenceNumber = command.SequenceNumber, sectionNumber = command.SectionNumber, pageId = command.NextPageId });
            }
        }

        [HttpGet("ClarificationSectionReview/{applicationId}/Sector/{pageId}")]
        public async Task<IActionResult> ReviewSectorAnswers(Guid applicationId, string pageId)
        {
            var userId = HttpContext.User.UserId();
            var viewModel = await _sectionReviewOrchestrator.GetSectorDetailsViewModel(new GetSectorDetailsRequest(applicationId, pageId, userId));
            return View("~/Views/ClarificationSectionReview/ReviewSectorAnswers.cshtml", viewModel);
        }

        [HttpPost("ClarificationSectionReview/{applicationId}/Sector/{pageId}")]
        public async Task<IActionResult> ReviewSectorAnswers(SubmitClarificationPageAnswerCommand command)
        {
            if (command.ClarificationRequired)
            {
                var userId = HttpContext.User.UserId();
                Func<Task<ClarifierSectorDetailsViewModel>> viewModelBuilder = () => _sectionReviewOrchestrator.GetSectorDetailsViewModel(new GetSectorDetailsRequest(command.ApplicationId, command.PageId, userId));

                return await ValidateAndUpdateSectorPageAnswer(command, viewModelBuilder, $"~/Views/ClarificationSectionReview/ReviewSectorAnswers.cshtml");
            }
            else
            {
                return RedirectToAction("ReviewPageAnswers", "ClarificationSectionReview", new
                {
                    applicationId = command.ApplicationId,
                    sequenceNumber = SequenceIds.DeliveringApprenticeshipTraining,
                    sectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees
                });
            }
        }


        [HttpGet("ClarificationSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}/Page/{pageId}/Download/{fileName}")]
        public async Task<IActionResult> DownloadClarificationFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string fileName)
        {
            var response = await _clarificationApiClient.DownloadFile(applicationId, sequenceNumber, sectionNumber, pageId, fileName);

            if (response.IsSuccessStatusCode)
            {
                var fileStream = await response.Content.ReadAsStreamAsync();

                return File(fileStream, response.Content.Headers.ContentType.MediaType, fileName);
            }

            return NotFound();
        }

        [HttpGet("ClarificationSectionReview/{applicationId}/Sequence/{sequenceNumber}/Section/{sectionNumber}/Page/{pageId}/Delete/{fileName}")]
        public async Task<IActionResult> DeleteClarificationFile(Guid applicationId, int sequenceNumber, int sectionNumber, string pageId, string fileName)
        {
            await _clarificationApiClient.DeleteFile(applicationId, sequenceNumber, sectionNumber, pageId, fileName);

            return RedirectToAction("ReviewPageAnswers", "ClarificationSectionReview", new { applicationId, sequenceNumber, sectionNumber, pageId });
        }
    }
}