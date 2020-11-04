using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class SubmitOutcomePageAnswerCommand
    {
        public Guid ApplicationId { get; set; }
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }
        public string NextPageId { get; set; }

        public SubmitOutcomePageAnswerCommand()
        {
        }

        public SubmitOutcomePageAnswerCommand(OutcomeReviewAnswersViewModel viewModel)
        {
            ApplicationId = viewModel.ApplicationId;
            PageId = viewModel.PageId;
            NextPageId = viewModel.NextPageId;
            SequenceNumber = viewModel.SequenceNumber;
            SectionNumber = viewModel.SectionNumber;
        }
    }
}
