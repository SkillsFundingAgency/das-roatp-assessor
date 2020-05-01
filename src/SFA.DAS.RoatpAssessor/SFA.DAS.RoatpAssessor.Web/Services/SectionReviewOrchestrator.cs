using Microsoft.Extensions.Logging;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public class SectionReviewOrchestrator : ISectionReviewOrchestrator
    {
        private readonly IRoatpApplicationApiClient _applyApiClient;
        private readonly ILogger<SectionReviewOrchestrator> _logger;

        public SectionReviewOrchestrator(IRoatpApplicationApiClient applyApiClient, ILogger<SectionReviewOrchestrator> logger)
        {

            _applyApiClient = applyApiClient;
            _logger = logger;
        }

        public async Task<ReviewAnswersViewModel> GetReviewAnswersViewModel(GetReviewAnswersRequest request)
        {
            var viewModel = new ReviewAnswersViewModel { ApplicationId = request.ApplicationId, SequenceNumber = request.SequenceNumber, SectionNumber = request.SectionNumber, PageId = request.PageId };
            return viewModel;
        }
    }
}
