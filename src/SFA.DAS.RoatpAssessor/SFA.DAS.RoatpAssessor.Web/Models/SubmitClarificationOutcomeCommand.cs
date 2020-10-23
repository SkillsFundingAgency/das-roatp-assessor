using System;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class SubmitClarificationOutcomeCommand
    {
        public Guid ApplicationId { get; set; }
        public string Status { get; set; }
        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }
        public string OptionInProgressText { get; set; }

        public string ReviewComment
        {
            get
            {
                string reviewComment;

                switch (Status)
                {
                    case ClarificationPageReviewStatus.Pass:
                        reviewComment = OptionPassText;
                        break;
                    case ClarificationPageReviewStatus.Fail:
                        reviewComment = OptionFailText;
                        break;
                    case ClarificationPageReviewStatus.InProgress:
                        reviewComment = OptionInProgressText;
                        break;
                    default:
                        reviewComment = null;
                        break;
                }

                return reviewComment;
            }
        }

        public SubmitClarificationOutcomeCommand()
        {

        }

        public SubmitClarificationOutcomeCommand(ClarificationOutcomeViewModel viewModel)
        {
            ApplicationId = viewModel.ApplicationId;
            Status = viewModel.Status;
            OptionPassText = viewModel.OptionPassText;
            OptionFailText = viewModel.OptionFailText;
            OptionInProgressText = viewModel.OptionInProgressText;
        }
    }
}