using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.AdminService.Common.Extensions;
using SFA.DAS.AdminService.Common.Testing.MockedObjects;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Consts;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.ClarificationSectionReviewOrchestrator
{
    [TestFixture]
    public class GetSectorsViewModelTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applicationApiClient;
        private Mock<IRoatpClarificationApiClient> _clarificationApiClient;
        private Web.Services.ClarificationSectionReviewOrchestrator _orchestrator;
        private string _clarificationPageCaption;
        private string _ukprn;
        private List<ClarificationSector> _chosenSectors;
        private string _organisationName;
        private string _providerRouteName;
        private readonly DateTime _applicationSubmittedOn = DateTime.Today;

        [SetUp]
        public void SetUp()
        {
            _clarificationPageCaption = "Caption for page";
            var logger = new Mock<ILogger<Web.Services.ClarificationSectionReviewOrchestrator>>();
            _chosenSectors = new List<ClarificationSector>();

            _applicationApiClient = new Mock<IRoatpApplicationApiClient>();
            _clarificationApiClient = new Mock<IRoatpClarificationApiClient>();

            var supplementaryInformationService = new Mock<ISupplementaryInformationService>();

            _orchestrator = new Web.Services.ClarificationSectionReviewOrchestrator(logger.Object, _applicationApiClient.Object, _clarificationApiClient.Object, supplementaryInformationService.Object);
        }

        [Test]
        public async Task GetSectorsViewModel_returns_ViewModel()
        {
            int sequenceNumber = SequenceIds.DeliveringApprenticeshipTraining;
            int sectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees;
            string pageId = RoatpWorkflowPageIds.YourSectorsAndEmployeesStartingPageId;
            var userId = _user.UserId();
            _chosenSectors.Add(new ClarificationSector { PageId = "1", Title = "page 1 title", Status = "Pass" });
            _chosenSectors.Add(new ClarificationSector { PageId = "2", Title = "page 2 title" });
            _ukprn = "1234";
            _organisationName = "org name";
            _providerRouteName = "Main";

            var application = new Apply
            {
                ApplicationId = _applicationId,
                ApplyData = new ApplyData
                {
                    ApplyDetails = new ApplyDetails
                    {
                        UKPRN = _ukprn,
                        OrganisationName = _organisationName,
                        ProviderRouteName = _providerRouteName,
                        ApplicationSubmittedOn = _applicationSubmittedOn
                    }
                }
            };

            var assessorPage = new ClarificationPage
            {
                Caption = _clarificationPageCaption
            };

            _applicationApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(application);

            _clarificationApiClient.Setup(x => x.GetClarificationPage(_applicationId, sequenceNumber, sectionNumber, pageId))
                .ReturnsAsync(assessorPage);

            _clarificationApiClient.Setup(x => x.GetClarificationSectors(_applicationId, userId))
                .ReturnsAsync(_chosenSectors);


            var request = new GetSectorsRequest(_applicationId, userId);
            var actualViewModel = await _orchestrator.GetSectorsViewModel(request);

            Assert.That(actualViewModel, Is.Not.Null);

            var expectedViewModel = new ApplicationSectorsViewModel
            {
                ApplyLegalName = _organisationName,
                Ukprn = _ukprn,
                SelectedSectors = _chosenSectors,
                ApplicationId = _applicationId,
                ApplicationRoute = _providerRouteName,
                SubmittedDate = _applicationSubmittedOn,
                Caption = _clarificationPageCaption,
                Heading = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployeesHeading
            };

            Assert.AreEqual(JsonConvert.SerializeObject(actualViewModel), JsonConvert.SerializeObject(expectedViewModel));
        }
    }
}

