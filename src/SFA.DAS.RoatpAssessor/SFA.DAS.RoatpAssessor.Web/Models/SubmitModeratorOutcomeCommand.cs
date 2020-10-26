using System;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class SubmitModeratorOutcomeCommand
    {
        public Guid ApplicationId { get; set; }
        public string Status { get; set; }
        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }
        public string OptionAskForClarificationText { get; set; }

        public string ReviewComment
        {
            get
            {
                string reviewComment;

                switch (Status)
                {
                    case ModeratorPageReviewStatus.Pass:
                        reviewComment = OptionPassText;
                        break;
                    case ModeratorPageReviewStatus.Fail:
                        reviewComment = OptionFailText;
                        break;
                    case ModeratorPageReviewStatus.AskForClarification:
                        reviewComment = OptionAskForClarificationText;
                        break;
                    default:
                        reviewComment = null;
                        break;
                }

                return reviewComment;
            }
        }

        public SubmitModeratorOutcomeCommand()
        {

        }

        public SubmitModeratorOutcomeCommand(ModeratorOutcomeViewModel viewModel)
        {
            ApplicationId = viewModel.ApplicationId;
            Status = viewModel.Status;
            OptionPassText = viewModel.OptionPassText;
            OptionFailText = viewModel.OptionFailText;
            OptionAskForClarificationText = viewModel.OptionAskForClarificationText;
        }
    }
}