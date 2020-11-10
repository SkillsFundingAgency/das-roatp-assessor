using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.Controllers.Outcome;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.OutcomeSectionReview
{
    [TestFixture]
    public class OutcomeSectionReviewControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private const int _sequenceNumber = 4;
        private const int _sectionNumber = 2;
        private const string _pageId = "4200";
        private const string _nextPageId = "4210";

        private Mock<IOutcomeSectionReviewOrchestrator> _sectionReviewOrchestrator;

        private OutcomeSectionReviewController _controller;


        [SetUp]
        public void SetUp()
        {
            _sectionReviewOrchestrator = new Mock<IOutcomeSectionReviewOrchestrator>();

            _controller = new OutcomeSectionReviewController(_sectionReviewOrchestrator.Object)
            {
                ControllerContext = MockedControllerContext.Setup()
            };
        }

        [Test]
        public async Task ReviewPageAnswers_When_FirstPageInSection()
        {
            var viewModel = new OutcomeReviewAnswersViewModel
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
            var viewModel = new OutcomeReviewAnswersViewModel
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
        public void POST_ReviewPageAnswers_when_no_NextPage_Redirects_To_Outcome_Overview()
        {
            var viewModel = new OutcomeReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = _sequenceNumber,
                SectionNumber = _sectionNumber,
                PageId = _pageId,
                NextPageId = null
            };

            var command = new SubmitOutcomePageAnswerCommand(viewModel);

            _sectionReviewOrchestrator.Setup(x => x.GetReviewAnswersViewModel(It.IsAny<GetReviewAnswersRequest>())).ReturnsAsync(viewModel);

            // act
            var result = _controller.ReviewPageAnswers(command) as RedirectToActionResult;

            // assert
            Assert.AreEqual("OutcomeOverview", result.ControllerName);
            Assert.AreEqual("ViewApplication", result.ActionName);
        }

        [Test]
        public void POST_ReviewPageAnswers_when_has_NextPage_Redirects_To_Next_Page()
        {
            var viewModel = new OutcomeReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = _sequenceNumber,
                SectionNumber = _sectionNumber,
                PageId = _pageId,
                NextPageId = _nextPageId
            };

            var command = new SubmitOutcomePageAnswerCommand(viewModel);

            _sectionReviewOrchestrator.Setup(x => x.GetReviewAnswersViewModel(It.IsAny<GetReviewAnswersRequest>())).ReturnsAsync(viewModel);

            // act
            var result = _controller.ReviewPageAnswers(command) as RedirectToActionResult;

            // assert
            Assert.AreEqual("OutcomeSectionReview", result.ControllerName);
            Assert.AreEqual("ReviewPageAnswers", result.ActionName);
        }
    }
}
