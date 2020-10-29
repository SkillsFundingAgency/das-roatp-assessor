using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using SFA.DAS.RoatpAssessor.Web.Controllers.Clarification;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.ClarificationSectionReview
{
    [TestFixture]
    public class ClarificationSectionReviewControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private const int _sequenceNumber = 4;
        private const int _sectionNumber = 2;
        private const string _pageId = "4200";
        private const string _nextPageId = "4210";

        private Mock<IRoatpClarificationApiClient> _clarificationApiClient;
        private Mock<IClarificationPageValidator> _clarificationPageValidator;
        private Mock<IClarificationSectionReviewOrchestrator> _sectionReviewOrchestrator;

        private ClarificationSectionReviewController _controller;


        [SetUp]
        public void SetUp()
        {
            _clarificationApiClient = new Mock<IRoatpClarificationApiClient>();
            _clarificationPageValidator = new Mock<IClarificationPageValidator>();
            _sectionReviewOrchestrator = new Mock<IClarificationSectionReviewOrchestrator>();

            var logger = Mock.Of<ILogger<ClarificationSectionReviewController>>();

            _controller = new ClarificationSectionReviewController(_clarificationApiClient.Object, _clarificationPageValidator.Object, _sectionReviewOrchestrator.Object, logger)
            {
                ControllerContext = MockedControllerContext.Setup()
            };
        }

        [Test]
        public async Task ReviewPageAnswers_When_FirstPageInSection()
        {
            var viewModel = new ClarifierReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = _sequenceNumber,
                SectionNumber = _sectionNumber,
                PageId = _pageId
            };

            _sectionReviewOrchestrator.Setup(x => x.GetReviewAnswersViewModel(It.IsAny<GetReviewAnswersRequest>())).ReturnsAsync(viewModel);

            // act
            var result = await _controller.ReviewPageAnswers(_applicationId, _sequenceNumber, _sectionNumber, null) as ViewResult;
            var actualViewModel = result?.Model as ReviewAnswersViewModel;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(viewModel));
        }


        [Test]
        public async Task ReviewPageAnswers_When_Sectors_Page_Invoked()
        {
            var sequenceNumber = SequenceIds.DeliveringApprenticeshipTraining;
            var sectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees;
            var chosenSectors = new List<ClarificationSector>
            {
                new ClarificationSector {PageId = "1", Title = "Page 1"},
                new ClarificationSector {PageId = "2", Title = "Page 2"}
            };

            var viewModel = new ApplicationSectorsViewModel
            {
                ApplicationId = _applicationId,
                SelectedSectors = chosenSectors
            };

            _sectionReviewOrchestrator.Setup(x => x.GetSectorsViewModel(It.IsAny<GetSectorsRequest>())).ReturnsAsync(viewModel);

            var result = await _controller.ReviewPageAnswers(_applicationId, sequenceNumber, sectionNumber, null) as ViewResult;
            var actualViewModel = result?.Model as ApplicationSectorsViewModel;

            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(viewModel));
        }

        [Test]
        public async Task ReviewPageAnswers_When_NotFirstPageInSequence()
        {
            var viewModel = new ClarifierReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = _sequenceNumber,
                SectionNumber = _sectionNumber,
                PageId = _pageId,
                NextPageId = _nextPageId
            };

            _sectionReviewOrchestrator.Setup(x => x.GetReviewAnswersViewModel(It.IsAny<GetReviewAnswersRequest>())).ReturnsAsync(viewModel);

            // act
            var result = await _controller.ReviewPageAnswers(_applicationId, _sequenceNumber, _sectionNumber, _pageId) as ViewResult;
            var actualViewModel = result?.Model as ReviewAnswersViewModel;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(viewModel));
        }

        [Test]
        public async Task POST_ReviewPageAnswers_When_Valid_submits_ClarificationPageReviewOutcome()
        {
            var viewModel = new ClarifierReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = _sequenceNumber,
                SectionNumber = _sectionNumber,
                PageId = _pageId,
                Status = ClarificationPageReviewStatus.Pass,
                OptionPassText = "test",
                ClarificationResponse = "All good"
            };

            var command = new SubmitClarificationPageAnswerCommand(viewModel);

            _sectionReviewOrchestrator.Setup(x => x.GetReviewAnswersViewModel(It.IsAny<GetReviewAnswersRequest>())).ReturnsAsync(viewModel);

            var validationResponse = new ValidationResponse();
            _clarificationPageValidator.Setup(x => x.Validate(command)).ReturnsAsync(validationResponse);

            _clarificationApiClient.Setup(x => x.SubmitClarificationPageReviewOutcome(command.ApplicationId,
                                    command.SequenceNumber,
                                    command.SectionNumber,
                                    command.PageId,
                                    _controller.User.UserId(),
                                    _controller.User.UserDisplayName(),
                                    command.ClarificationResponse,
                                    command.Status,
                                    command.ReviewComment,
                                    It.IsAny<IFormFileCollection>())).ReturnsAsync(true);

            // act
            var result = await _controller.ReviewPageAnswers(command) as RedirectToActionResult;

            // assert
            Assert.AreEqual("ClarificationOverview", result.ControllerName);
            Assert.AreEqual("ViewApplication", result.ActionName);

            _clarificationApiClient.Verify(x => x.SubmitClarificationPageReviewOutcome(command.ApplicationId,
                        command.SequenceNumber,
                        command.SectionNumber,
                        command.PageId,
                        _controller.User.UserId(),
                        _controller.User.UserDisplayName(),
                        command.ClarificationResponse,
                        command.Status,
                        command.ReviewComment,
                        It.IsAny<IFormFileCollection>()), Times.Once);
        }

        [Test]
        public async Task POST_ReviewPageAnswers_When_Invalid_does_not_submit_ClarificationPageReviewOutcome()
        {
            var viewModel = new ClarifierReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = _sequenceNumber,
                SectionNumber = _sectionNumber,
                PageId = _pageId,
                Status = ClarificationPageReviewStatus.Pass,
                OptionPassText = "test",
                ClarificationResponse = null
            };

            var command = new SubmitClarificationPageAnswerCommand(viewModel);

            _sectionReviewOrchestrator.Setup(x => x.GetReviewAnswersViewModel(It.IsAny<GetReviewAnswersRequest>())).ReturnsAsync(viewModel);

            var error = new ValidationErrorDetail { Field = "Status", ErrorMessage = "Error" };
            var validationResponse = new ValidationResponse { Errors = new List<ValidationErrorDetail> { error } };
            _clarificationPageValidator.Setup(x => x.Validate(command)).ReturnsAsync(validationResponse);

            // act
            var result = await _controller.ReviewPageAnswers(command) as ViewResult;
            var actualViewModel = result?.Model as ReviewAnswersViewModel;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(viewModel));

            _clarificationApiClient.Verify(x => x.SubmitClarificationPageReviewOutcome(command.ApplicationId,
                        command.SequenceNumber,
                        command.SectionNumber,
                        command.PageId,
                        _controller.User.UserId(),
                        _controller.User.UserDisplayName(),
                        command.ClarificationResponse,
                        command.Status,
                        command.ReviewComment,
                        It.IsAny<IFormFileCollection>()), Times.Never);
        }


        [Test]
        public async Task POST_ReviewPageAnswers_When_ClarificationRequired_is_false_redirects_to_NextPage()
        {
            var viewModel = new ClarifierReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = _sequenceNumber,
                SectionNumber = _sectionNumber,
                PageId = _pageId,
                Status = ClarificationPageReviewStatus.Pass,
                OptionPassText = "test",
                ClarificationResponse = "All good",
                NextPageId = _nextPageId
            };

            var command = new SubmitClarificationPageAnswerCommand(viewModel);
            command.ClarificationRequired = false;

            _sectionReviewOrchestrator.Setup(x => x.GetReviewAnswersViewModel(It.IsAny<GetReviewAnswersRequest>())).ReturnsAsync(viewModel);

            // act
            var result = await _controller.ReviewPageAnswers(command) as RedirectToActionResult;

            // assert
            Assert.AreEqual("ClarificationSectionReview", result.ControllerName);
            Assert.AreEqual("ReviewPageAnswers", result.ActionName);

            _clarificationApiClient.Verify(x => x.SubmitClarificationPageReviewOutcome(command.ApplicationId,
                        command.SequenceNumber,
                        command.SectionNumber,
                        command.PageId,
                        _controller.User.UserId(),
                        _controller.User.UserDisplayName(),
                        command.ClarificationResponse,
                        command.Status,
                        command.ReviewComment,
                        It.IsAny<IFormFileCollection>()), Times.Never);
        }

        [Test]
        public async Task DownloadClarificationFile_when_file_exists_downloads_the_requested_file()
        {
            string filename = "test.pdf";
            string contentType = "application/pdf";

            var response = new HttpResponseMessage(HttpStatusCode.OK);
            response.Content = new StreamContent(new MemoryStream())
            { Headers = { ContentLength = 0, ContentType = new MediaTypeHeaderValue(contentType) } };

            _clarificationApiClient.Setup(x => x.DownloadFile(_applicationId, _sequenceNumber, _sectionNumber, _pageId, filename)).ReturnsAsync(response);

            // act
            var result = await _controller.DownloadClarificationFile(_applicationId, _sequenceNumber, _sectionNumber, _pageId, filename) as FileStreamResult;

            // assert
            Assert.IsNotNull(result);
            Assert.AreEqual(filename, result.FileDownloadName);
            Assert.AreEqual(contentType, result.ContentType);
        }

        [Test]
        public async Task DownloadClarificationFile_when_file_does_not_exists_then_gives_NotFound_result()
        {
            string filename = "test.pdf";

            var response = new HttpResponseMessage(HttpStatusCode.NotFound);

            _clarificationApiClient.Setup(x => x.DownloadFile(_applicationId, _sequenceNumber, _sectionNumber, _pageId, filename)).ReturnsAsync(response);

            // act
            var result = await _controller.DownloadClarificationFile(_applicationId, _sequenceNumber, _sectionNumber, _pageId, filename) as NotFoundResult;

            // assert
            Assert.IsNotNull(result);
        }

        [Test]
        public async Task DeleteClarificationFile_deletes_the_file_and_redirects_to_ReviewPageAnswers()
        {
            string filename = "test.pdf";

            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            _clarificationApiClient.Setup(x => x.DeleteFile(_applicationId, _sequenceNumber, _sectionNumber, _pageId, filename)).ReturnsAsync(response);

            // act
            var result = await _controller.DeleteClarificationFile(_applicationId, _sequenceNumber, _sectionNumber, _pageId, filename) as RedirectToActionResult;

            // assert
            Assert.AreEqual("ClarificationSectionReview", result.ControllerName);
            Assert.AreEqual("ReviewPageAnswers", result.ActionName);
        }
    }
}
