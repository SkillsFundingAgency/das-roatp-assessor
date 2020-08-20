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
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Consts;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.Services;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Services.SectionReviewOrchestrator
{
    [TestFixture]
    public class GetSectorsViewModelTests
    {
        private readonly Guid _applicationId = Guid.NewGuid();
        private readonly ClaimsPrincipal _user = MockedUser.Setup();

        private Mock<IRoatpApplicationApiClient> _applicationApiClient;
        private Mock<IRoatpAssessorApiClient> _assessorApiClient;
        private Web.Services.SectionReviewOrchestrator _orchestrator;
        private string _assessorPageCaption;
        private string _ukprn;
        private List<Sector> _chosenSectors;
        private string _organisationName;
        private string _providerRouteName;
        private readonly DateTime _applicationSubmittedOn = DateTime.Today;

        [SetUp]
        public void SetUp()
        {
            _assessorPageCaption = "Caption for page";
            var logger = new Mock<ILogger<Web.Services.SectionReviewOrchestrator>>();
            _chosenSectors = new List<Sector>();

            _applicationApiClient = new Mock<IRoatpApplicationApiClient>();
            _assessorApiClient = new Mock<IRoatpAssessorApiClient>();

            var supplementaryInformationService = new Mock<ISupplementaryInformationService>();

            _orchestrator = new Web.Services.SectionReviewOrchestrator(logger.Object, _applicationApiClient.Object, _assessorApiClient.Object, supplementaryInformationService.Object);
        }

        [Test]
        public async Task GetSectorsViewModel_returns_ViewModel()
        {
            int sequenceNumber = SequenceIds.DeliveringApprenticeshipTraining;
            int sectionNumber = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployees;
            string pageId = RoatpWorkflowPageIds.YourSectorsAndEmployeesStartingPageId;
            var userId = _user.UserId();
            _chosenSectors.Add(new Sector { PageId = "1", Title = "page 1 title", Status = "Pass" });
            _chosenSectors.Add(new Sector { PageId = "2", Title = "page 2 title" });
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

            var assessorPage = new AssessorPage
            {
                Caption = _assessorPageCaption
            };

            _applicationApiClient.Setup(x => x.GetApplication(_applicationId)).ReturnsAsync(application);

            _assessorApiClient.Setup(x => x.GetAssessorPage(_applicationId, sequenceNumber, sectionNumber, pageId))
                .ReturnsAsync(assessorPage);

            _assessorApiClient.Setup(x => x.GetAssessorSectors(_applicationId, userId))
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
                Caption = _assessorPageCaption,
                Heading = SectionIds.DeliveringApprenticeshipTraining.YourSectorsAndEmployeesHeading
            };

            Assert.AreEqual(JsonConvert.SerializeObject(actualViewModel), JsonConvert.SerializeObject(expectedViewModel));
        }
    }
}

