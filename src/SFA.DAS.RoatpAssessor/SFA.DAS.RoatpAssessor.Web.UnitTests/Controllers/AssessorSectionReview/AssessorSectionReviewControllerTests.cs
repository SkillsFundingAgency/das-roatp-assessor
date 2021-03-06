﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using SFA.DAS.RoatpAssessor.Web.Controllers.Assessor;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.AssessorSectionReview
{
    [TestFixture]
    public class AssessorSectionReviewControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();

        private Mock<IRoatpAssessorApiClient> _assessorApiClient;
        private Mock<IAssessorPageValidator> _assessorPageValidator;
        private Mock<IAssessorSectionReviewOrchestrator> _sectionReviewOrchestrator;

        private AssessorSectionReviewController _controller;


        [SetUp]
        public void SetUp()
        {
            _assessorApiClient = new Mock<IRoatpAssessorApiClient>();
            _assessorPageValidator = new Mock<IAssessorPageValidator>();
            _sectionReviewOrchestrator = new Mock<IAssessorSectionReviewOrchestrator>();

            var logger = Mock.Of<ILogger<AssessorSectionReviewController>>();

            _controller = new AssessorSectionReviewController(_assessorApiClient.Object, _assessorPageValidator.Object, _sectionReviewOrchestrator.Object, logger)
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

            var viewModel = new AssessorReviewAnswersViewModel
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
            var chosenSectors = new List<AssessorSector>
            {
                new AssessorSector {PageId = "1", Title = "Page 1"},
                new AssessorSector {PageId = "2", Title = "Page 2"}
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

            var viewModel = new AssessorReviewAnswersViewModel
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
        public async Task POST_ReviewPageAnswers_When_Valid_submits_AssessorPageReviewOutcome()
        {
            int sequenceNumber = 4;
            int sectionNumber = 2;
            string pageId = "4200";

            var viewModel = new AssessorReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId,
                Status = AssessorPageReviewStatus.Pass,
                OptionPassText = "test"
            };

            var command = new SubmitAssessorPageAnswerCommand(viewModel);

            _sectionReviewOrchestrator.Setup(x => x.GetReviewAnswersViewModel(It.IsAny<GetReviewAnswersRequest>())).ReturnsAsync(viewModel);

            var validationResponse = new ValidationResponse();
            _assessorPageValidator.Setup(x => x.Validate(command)).ReturnsAsync(validationResponse);

            _assessorApiClient.Setup(x => x.SubmitAssessorPageReviewOutcome(command.ApplicationId,
                                    command.SequenceNumber,
                                    command.SectionNumber,
                                    command.PageId,
                                    _controller.User.UserId(),
                                    _controller.User.UserDisplayName(),
                                    command.Status,
                                    command.ReviewComment)).ReturnsAsync(true);

            // act
            var result = await _controller.ReviewPageAnswers(_applicationId, sequenceNumber, sectionNumber, pageId, command) as RedirectToActionResult;

            // assert
            Assert.AreEqual("AssessorOverview", result.ControllerName);
            Assert.AreEqual("ViewApplication", result.ActionName);

            _assessorApiClient.Verify(x => x.SubmitAssessorPageReviewOutcome(command.ApplicationId,
                        command.SequenceNumber,
                        command.SectionNumber,
                        command.PageId,
                        _controller.User.UserId(),
                        _controller.User.UserDisplayName(),
                        command.Status,
                        command.ReviewComment), Times.Once);
        }

        [Test]
        public async Task POST_ReviewPageAnswers_When_Invalid_does_not_submit_AssessorPageReviewOutcome()
        {
            int sequenceNumber = 4;
            int sectionNumber = 2;
            string pageId = "4200";

            var viewModel = new AssessorReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId,
                Status = AssessorPageReviewStatus.Pass,
                OptionPassText = "test"
            };

            var command = new SubmitAssessorPageAnswerCommand(viewModel);

            _sectionReviewOrchestrator.Setup(x => x.GetReviewAnswersViewModel(It.IsAny<GetReviewAnswersRequest>())).ReturnsAsync(viewModel);

            var error = new ValidationErrorDetail { Field = "Status", ErrorMessage = "Error" };
            var validationResponse = new ValidationResponse { Errors = new List<ValidationErrorDetail> { error } };
            _assessorPageValidator.Setup(x => x.Validate(command)).ReturnsAsync(validationResponse);

            // act
            var result = await _controller.ReviewPageAnswers(_applicationId, sequenceNumber, sectionNumber, pageId, command) as ViewResult;
            var actualViewModel = result?.Model as ReviewAnswersViewModel;

            // assert
            Assert.That(result, Is.Not.Null);
            Assert.That(actualViewModel, Is.Not.Null);
            Assert.That(actualViewModel, Is.SameAs(viewModel));

            _assessorApiClient.Verify(x => x.SubmitAssessorPageReviewOutcome(command.ApplicationId,
                        command.SequenceNumber,
                        command.SectionNumber,
                        command.PageId,
                        _controller.User.UserId(),
                        _controller.User.UserDisplayName(),
                        command.Status,
                        command.ReviewComment), Times.Never);
        }
    }
}
