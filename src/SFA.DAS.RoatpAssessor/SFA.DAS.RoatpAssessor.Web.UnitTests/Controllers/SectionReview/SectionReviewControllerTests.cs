using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Controllers;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.UnitTests.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.Validators;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Controllers.SectionReview
{
    [TestFixture]
    public class SectionReviewControllerTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();

        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Mock<IRoatpAssessorPageValidator> _assessorPageValidator;
        private Mock<ISectionReviewOrchestrator> _sectionReviewOrchestrator;

        private SectionReviewController _controller;
        

        [SetUp]
        public void SetUp()
        {
            _applyApiClient = new Mock<IRoatpApplicationApiClient>();
            _assessorPageValidator = new Mock<IRoatpAssessorPageValidator>();
            _sectionReviewOrchestrator = new Mock<ISectionReviewOrchestrator>();

            var logger = Mock.Of<ILogger<SectionReviewController>>();

            _controller = new SectionReviewController(_applyApiClient.Object, _assessorPageValidator.Object, _sectionReviewOrchestrator.Object, logger)
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

            var viewModel = new ReviewAnswersViewModel
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
        public async Task ReviewPageAnswers_When_NotFirstPageInSequence()
        {
            int sequenceNumber = 4;
            int sectionNumber = 2;
            string pageId = "4200";

            var viewModel = new ReviewAnswersViewModel
            {
                ApplicationId = _applicationId,
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId
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
    }
}
