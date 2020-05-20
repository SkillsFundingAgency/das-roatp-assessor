﻿using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.SectionReviewOrchestrator
{
    [TestFixture]
    public class GetReviewAnswersViewModelTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applyApiClient;
        private Web.Services.SectionReviewOrchestrator _orchestrator;

        [SetUp]
        public void SetUp()
        {
            var logger = new Mock<ILogger<Web.Services.SectionReviewOrchestrator>>();

            _applyApiClient = new Mock<IRoatpApplicationApiClient>();

            var supplementaryInformationService = new Mock<ISupplementaryInformationService>();

            _orchestrator = new Web.Services.SectionReviewOrchestrator(logger.Object, _applyApiClient.Object, supplementaryInformationService.Object);
        }

        [Test]
        public async Task GetReviewAnswersViewModel_returns_ViewModel()
        {
            int sequenceNumber = 4;
            int sectionNumber = 2;
            string pageId = "4200";
            var userId = _user.UserId();

            var application = new Apply
            {
                ApplicationId = _applicationId,
                ApplyData = new ApplyData
                {
                    ApplyDetails = new ApplyDetails { }
                }
            };

            var contact = new Contact
            {
                Email = userId,
                GivenNames = _user.GivenName(),
                FamilyName = _user.Surname()
            };

            var assessorPage = new AssessorPage
            {
                ApplicationId = _applicationId,
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId,
                Questions = new List<AssessorQuestion>
                {
                    new AssessorQuestion { QuestionId = "Q1" }
                },
                Answers = new List<AssessorAnswer>
                {
                    new AssessorAnswer { QuestionId = "Q1", Value = "value" }
                }
            };

            var pageReviewOutcome = new PageReviewOutcome
            {
                ApplicationId = _applicationId,
                SequenceNumber = sequenceNumber,
                SectionNumber = sectionNumber,
                PageId = pageId,
                UserId = userId,
                Status = AssessorPageReviewStatus.Pass
            };

            _applyApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(application);

            _applyApiClient.Setup(x => x.GetContactForApplication(_applicationId)).ReturnsAsync(contact);

            _applyApiClient.Setup(x => x.GetAssessorPage(_applicationId, sequenceNumber, sectionNumber, pageId))
                .ReturnsAsync(assessorPage);

            _applyApiClient.Setup(x => x.GetAssessorReviewOutcomesPerSection(_applicationId, sequenceNumber, sectionNumber, It.IsAny<int>(), userId))
                .ReturnsAsync(new List<PageReviewOutcome> { pageReviewOutcome });

            _applyApiClient.Setup(x => x.GetPageReviewOutcome(_applicationId, sequenceNumber, sectionNumber, pageId, It.IsAny<int>(), userId))
                .ReturnsAsync(pageReviewOutcome);

            var request = new GetReviewAnswersRequest(_applicationId, userId, sequenceNumber, sectionNumber, pageId, null);
            var result = await _orchestrator.GetReviewAnswersViewModel(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.ApplicationId, Is.EqualTo(assessorPage.ApplicationId));
            Assert.That(result.SequenceNumber, Is.EqualTo(assessorPage.SequenceNumber));
            Assert.That(result.SectionNumber, Is.EqualTo(assessorPage.SectionNumber));
            Assert.That(result.PageId, Is.EqualTo(assessorPage.PageId));
            Assert.That(result.Status, Is.EqualTo(pageReviewOutcome.Status));
            CollectionAssert.IsNotEmpty(result.Questions);
            CollectionAssert.IsNotEmpty(result.Answers);
        }
    }
}
