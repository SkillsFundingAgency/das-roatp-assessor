using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Services
{
    public interface ISectionReviewOrchestrator
    {
        Task<ReviewAnswersViewModel> GetReviewAnswersViewModel(GetReviewAnswersRequest request);
        //Task<ReviewAnswersViewModel> GetNextPageReviewAnswersViewModel(GetReviewAnswersRequest request);
    }
}
