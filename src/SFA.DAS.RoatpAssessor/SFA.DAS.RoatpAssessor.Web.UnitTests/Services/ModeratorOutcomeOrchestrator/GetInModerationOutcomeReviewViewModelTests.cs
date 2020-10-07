using System;
using System.Collections.Generic;
using System.Security.Claims;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.ModeratorOutcomeOrchestrator
{
    [TestFixture]
    public class GetInModerationOutcomeReviewViewModelTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applicationApiClient;
        private Mock<IRoatpModerationApiClient> _moderationApiClient;
        private Web.Services.ModeratorOutcomeOrchestrator _orchestrator;

        private string Status => "Status";
        private string ReviewComment => "Review comments";


        private string _userId => _user.UserId();
        private string _userDisplayName => _user.UserDisplayName();
        private Apply _application;
        private Contact _contact;
        private List<ModeratorPageReviewOutcome> _outcomes;


        private string ApplicationRouteName => "Main";
        private string Ukprn => "23456789";
        private string OrganisationName => "Emporium Glorium";
        private DateTime ApplicationSubmittedOn => new DateTime(2020, 09, 30);
        private string Email => "email@address.com";
        private ModeratorOutcomeReviewViewModel _expectedViewModel;
        private ReviewModeratorOutcomeRequest _request;
        [SetUp]
        public void SetUp()
        {
            _applicationApiClient = new Mock<IRoatpApplicationApiClient>();
            _moderationApiClient = new Mock<IRoatpModerationApiClient>();
            _orchestrator = new Web.Services.ModeratorOutcomeOrchestrator(_applicationApiClient.Object, _moderationApiClient.Object);
            _application = new Apply
            {
                ApplicationId = _applicationId,
                ModerationStatus = ModerationStatus.New,
                Assessor1ReviewStatus = AssessorReviewStatus.Approved,
                Assessor1UserId = _userId,
                Assessor1Name = _userDisplayName,
                Assessor2ReviewStatus = AssessorReviewStatus.Approved,
                Assessor2UserId = $"{ _userId }-2",
                Assessor2Name = $"{ _userDisplayName }-2",
                ApplyData = new ApplyData
                {
                    ApplyDetails = new ApplyDetails
                    {
                        ProviderRouteName = ApplicationRouteName,
                        UKPRN = Ukprn,
                        OrganisationName = OrganisationName,
                        ApplicationSubmittedOn = ApplicationSubmittedOn
                    }
                }
            };

            _outcomes = new List<ModeratorPageReviewOutcome>();
            _contact = new Contact { Email = Email};
            _request = new ReviewModeratorOutcomeRequest(_applicationId, _userId, Status, ReviewComment);
            _applicationApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(_application);
            _applicationApiClient.Setup(x => x.GetContactForApplication(_applicationId)).ReturnsAsync(_contact);

            _expectedViewModel = new ModeratorOutcomeReviewViewModel
            {
                Status = Status,
                ApplicantEmailAddress = Email,
                Ukprn = Ukprn,
                ReviewComment = ReviewComment,
                ApplyLegalName = OrganisationName,
                ApplicationRoute = ApplicationRouteName,
                SubmittedDate = ApplicationSubmittedOn
            };
        }

        [Test]
        public void Return_null_view_model_if_no_application()
        {
            _applicationApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync((Apply)null);
            var result =  _orchestrator.GetInModerationOutcomeReviewViewModel(_request);
            Assert.IsNull(result.Result);
        }

        [Test]
        public void Return_null_view_model_if_no_contact()
        {
            _applicationApiClient.Setup(x => x.GetContactForApplication(_applicationId)).ReturnsAsync((Contact)null);
            var result = _orchestrator.GetInModerationOutcomeReviewViewModel(_request);
            Assert.IsNull(result.Result);
        }

        [Test]
        public void Return_view_model_if_successful()
        {

            var result = _orchestrator.GetInModerationOutcomeReviewViewModel(_request);
            var actualViewModel = result?.Result;

            Assert.AreEqual(JsonConvert.SerializeObject(actualViewModel), JsonConvert.SerializeObject(_expectedViewModel));
        }
    }
}
