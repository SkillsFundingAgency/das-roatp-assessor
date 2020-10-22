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
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.ClarificationSectionReview
{
    [TestFixture]
    public class ClarificationSectionReviewControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();

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
            int sequenceNumber = 4;
            int sectionNumber = 2;
            string pageId = "4200";

            var viewModel = new ClarifierReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId
            };

            _sectionReviewOrchestrator.Setup(x => x.GetReviewAnswersViewModel(It.IsAny<GetReviewAnswersRequest>())).ReturnsAsync(viewModel);

            // act
            var result = await _controller.ReviewPageAnswers(_applicationId, sequenceNumber, sectionNumber, null) as ViewResult;
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
            int sequenceNumber = 4;
            int sectionNumber = 2;
            string pageId = "4200";
            string nextPageId = "4210";

            var viewModel = new ClarifierReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId,
                NextPageId = nextPageId
            };

            _sectionReviewOrchestrator.Setup(x => x.GetReviewAnswersViewModel(It.IsAny<GetReviewAnswersRequest>())).ReturnsAsync(viewModel);

            // act
            var result = await _controller.ReviewPageAnswers(_applicationId, sequenceNumber, sectionNumber, pageId) as ViewResult;
            var actualViewModel = result?.Model as ReviewAnswersViewModel;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(viewModel));
        }

        [Test]
        public async Task POST_ReviewPageAnswers_When_Valid_submits_ClarificationPageReviewOutcome()
        {
            int sequenceNumber = 4;
            int sectionNumber = 2;
            string pageId = "4200";

            var viewModel = new ClarifierReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId,
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
                                    command.ClarificationResponse,
                                    command.Status,
                                    command.ReviewComment)).ReturnsAsync(true);

            // act
            var result = await _controller.ReviewPageAnswers(_applicationId, sequenceNumber, sectionNumber, pageId, command) as RedirectToActionResult;

            // assert
            Assert.AreEqual("ClarificationOverview", result.ControllerName);
            Assert.AreEqual("ViewApplication", result.ActionName);

            _clarificationApiClient.Verify(x => x.SubmitClarificationPageReviewOutcome(command.ApplicationId,
                        command.SequenceNumber,
                        command.SectionNumber,
                        command.PageId,
                        _controller.User.UserId(),
                        command.ClarificationResponse,
                        command.Status,
                        command.ReviewComment), Times.Once);
        }

        [Test]
        public async Task POST_ReviewPageAnswers_When_Invalid_does_not_submit_ClarificationPageReviewOutcome()
        {
            int sequenceNumber = 4;
            int sectionNumber = 2;
            string pageId = "4200";

            var viewModel = new ClarifierReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId,
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
            var result = await _controller.ReviewPageAnswers(_applicationId, sequenceNumber, sectionNumber, pageId, command) as ViewResult;
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
                        command.ClarificationResponse,
                        command.Status,
                        command.ReviewComment), Times.Never);
        }
    }
}
