using System;
using System.Collections.Generic;
using System.Security.Claims;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.ClarificationOutcomeOrchestrator
{
    [TestFixture]
    public class GetClarificationOutcomeViewModelTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private string _userId => _user.UserId();
        private string _userDisplayName => _user.UserDisplayName();

        private Apply _application;
        private Contact _contact;
        private List<ClarificationPageReviewOutcome> _outcomes;
        private GetClarificationOutcomeRequest _request;

        private ClarificationOutcomeViewModel _expectedOutcomeViewModel;

        private Mock<IRoatpApplicationApiClient> _applicationApiClient;
        private Mock<IRoatpClarificationApiClient> _clarificationApiClient;

        private Web.Services.ClarificationOutcomeOrchestrator _orchestrator;

        [SetUp]
        public void SetUp()
        {
            _applicationApiClient = new Mock<IRoatpApplicationApiClient>();
            _clarificationApiClient = new Mock<IRoatpClarificationApiClient>();
            _orchestrator = new Web.Services.ClarificationOutcomeOrchestrator(_applicationApiClient.Object, _clarificationApiClient.Object);
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
                        ProviderRouteName = "Main",
                        UKPRN = "23456789",
                        OrganisationName = "Emporium Glorium",
                        ApplicationSubmittedOn = DateTime.UtcNow
                    }
                }
            };

            _outcomes = new List<ClarificationPageReviewOutcome>();
            _contact = new Contact { Email = "email@address.com" };
            _request = new GetClarificationOutcomeRequest(_applicationId,_userId);
            _applicationApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(_application);
            _applicationApiClient.Setup(x => x.GetContactForApplication(_applicationId)).ReturnsAsync(_contact);
            _clarificationApiClient.Setup(x => x.GetAllClarificationPageReviewOutcomes(_applicationId, _userId)).ReturnsAsync(_outcomes);

            _expectedOutcomeViewModel = new ClarificationOutcomeViewModel(_application, _outcomes);
        }


        [Test]
        public void Return_null_view_model_if_no_application()
        {
            _applicationApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync((Apply)null);
            var result =  _orchestrator.GetClarificationOutcomeViewModel(_request);
            Assert.IsNull(result.Result);
        }

        [Test]
        public void Return_null_view_model_if_no_contact()
        {
            _applicationApiClient.Setup(x => x.GetContactForApplication(_applicationId)).ReturnsAsync((Contact)null);
            var result = _orchestrator.GetClarificationOutcomeViewModel(_request);
            Assert.IsNull(result.Result);
        }

        [Test]
        public void Return_null_view_model_if_more_than_one_outcome_not_pass_or_fail()
        {
            _outcomes = new List<ClarificationPageReviewOutcome>();
            _outcomes.Add(new ClarificationPageReviewOutcome { ApplicationId = _applicationId, Status = "not pass or fail" });
            _clarificationApiClient.Setup(x => x.GetAllClarificationPageReviewOutcomes(_applicationId, _userId)).ReturnsAsync(_outcomes);
            var result = _orchestrator.GetClarificationOutcomeViewModel(_request);
            Assert.IsNull(result.Result);
        }

        [Test]
        public void Return_view_model_with_pass_and_fail_counts_as_zero_if_no_outcomes_added()
        {
            _outcomes = new List<ClarificationPageReviewOutcome>();
            _clarificationApiClient.Setup(x => x.GetAllClarificationPageReviewOutcomes(_applicationId, _userId)).ReturnsAsync(_outcomes);
            var result = _orchestrator.GetClarificationOutcomeViewModel(_request);
            var actualViewModel = result?.Result;

            Assert.AreEqual(JsonConvert.SerializeObject(actualViewModel), JsonConvert.SerializeObject(_expectedOutcomeViewModel));
        }

        [Test]
        public void Return_view_model_with_pass_and_fail_counts_as_expected_if_outcomes_added()
        {
            _outcomes = new List<ClarificationPageReviewOutcome>();
            _outcomes.Add(new ClarificationPageReviewOutcome { Status = "Pass" });
            _outcomes.Add(new ClarificationPageReviewOutcome { Status = "Fail" });
            _outcomes.Add(new ClarificationPageReviewOutcome { Status = "Fail" });

            _clarificationApiClient.Setup(x => x.GetAllClarificationPageReviewOutcomes(_applicationId, _userId)).ReturnsAsync(_outcomes);
            var result = _orchestrator.GetClarificationOutcomeViewModel(_request);
            var actualViewModel = result?.Result;

            _expectedOutcomeViewModel.FailCount = 2;
            _expectedOutcomeViewModel.PassCount = 1;
            Assert.AreEqual(JsonConvert.SerializeObject(actualViewModel), JsonConvert.SerializeObject(_expectedOutcomeViewModel));
        }
    }
}
