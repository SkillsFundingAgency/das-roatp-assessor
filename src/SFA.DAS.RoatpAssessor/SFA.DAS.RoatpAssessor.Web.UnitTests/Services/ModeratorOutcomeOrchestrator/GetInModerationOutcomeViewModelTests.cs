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
    public class GetInModerationOutcomeViewModelTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applicationApiClient;
        private Mock<IRoatpModerationApiClient> _moderationApiClient;
        private Web.Services.ModeratorOutcomeOrchestrator _orchestrator;
        private string _userId => _user.UserId();
        private string _userDisplayName => _user.UserDisplayName();
        private Apply _application;
        private Contact _contact;
        private List<ModeratorPageReviewOutcome> _outcomes;
        private GetModeratorOutcomeRequest _request;
        private string ApplicationRouteName => "Main";
        private string Ukprn => "23456789";
        private string OrganisationName => "Emporium Glorium";
        private DateTime ApplicationSubmittedOn => new DateTime(2020, 09, 30);
        private string Email => "email@address.com";
        private ModeratorOutcomeViewModel _expectedOutcomeViewModel;
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
            _request = new GetModeratorOutcomeRequest(_applicationId,_userId);
            _applicationApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(_application);
            _applicationApiClient.Setup(x => x.GetContactForApplication(_applicationId)).ReturnsAsync(_contact);
            _moderationApiClient.Setup(x => x.GetAllModeratorPageReviewOutcomes(_applicationId, _userId)).ReturnsAsync(_outcomes);

            _expectedOutcomeViewModel = new ModeratorOutcomeViewModel(_application,_userId);
        }


        [Test]
        public void Return_null_view_model_if_no_application()
        {
            _applicationApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync((Apply)null);
            var result =  _orchestrator.GetInModerationOutcomeViewModel(_request);
            Assert.IsNull(result.Result);
        }

        [Test]
        public void Return_null_view_model_if_no_contact()
        {
            _applicationApiClient.Setup(x => x.GetContactForApplication(_applicationId)).ReturnsAsync((Contact)null);
            var result = _orchestrator.GetInModerationOutcomeViewModel(_request);
            Assert.IsNull(result.Result);
        }



        [Test]
        public void Return_null_view_model_if_more_than_one_outcome_not_pass_or_fail()
        {
            _outcomes = new List<ModeratorPageReviewOutcome>();
            _outcomes.Add(new ModeratorPageReviewOutcome {ApplicationId = _applicationId, Status = "not pass or fail"});
            _moderationApiClient.Setup(x => x.GetAllModeratorPageReviewOutcomes(_applicationId, _userId)).ReturnsAsync(_outcomes);
            var result = _orchestrator.GetInModerationOutcomeViewModel(_request);
            Assert.IsNull(result.Result);
        }

        [Test]
        public void Return_view_model_with_pass_and_fail_counts_as_zero_if_no_outcomes_added()
        {
            _outcomes = new List<ModeratorPageReviewOutcome>(); 
            _moderationApiClient.Setup(x => x.GetAllModeratorPageReviewOutcomes(_applicationId, _userId)).ReturnsAsync(_outcomes);
            var result = _orchestrator.GetInModerationOutcomeViewModel(_request);
            var actualViewModel = result?.Result;

            Assert.AreEqual(JsonConvert.SerializeObject(actualViewModel), JsonConvert.SerializeObject(_expectedOutcomeViewModel));
        }

        [Test]
        public void Return_view_model_with_pass_and_fail_counts_as_expected_if_outcomes_added()
        {
            _outcomes = new List<ModeratorPageReviewOutcome>();
            _outcomes.Add(new ModeratorPageReviewOutcome {Status="Pass"});
            _outcomes.Add(new ModeratorPageReviewOutcome { Status = "Pass" });
            _outcomes.Add(new ModeratorPageReviewOutcome { Status = "Fail" });

            _moderationApiClient.Setup(x => x.GetAllModeratorPageReviewOutcomes(_applicationId, _userId)).ReturnsAsync(_outcomes);
            var result = _orchestrator.GetInModerationOutcomeViewModel(_request);
            var actualViewModel = result?.Result;

            _expectedOutcomeViewModel.FailCount = 1;
            _expectedOutcomeViewModel.PassCount = 2;
            Assert.AreEqual(JsonConvert.SerializeObject(actualViewModel), JsonConvert.SerializeObject(_expectedOutcomeViewModel));
        }
    }
}
