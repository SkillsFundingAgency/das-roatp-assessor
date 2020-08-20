using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class SubmitAssessorPageAnswerCommand
    {
        public Guid ApplicationId { get; set; }
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }
        public string NextPageId { get; set; }
        public string Status { get; set; }
        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }
        public string OptionInProgressText { get; set; }
        public string Heading { get; set; }

        public SubmitAssessorPageAnswerCommand()
        {
                
        }

        public SubmitAssessorPageAnswerCommand(ReviewAnswersViewModel viewModel)
        {
            ApplicationId = viewModel.ApplicationId;
            PageId = viewModel.PageId;
            NextPageId = viewModel.NextPageId;
            SequenceNumber = viewModel.SequenceNumber;
            SectionNumber = viewModel.SectionNumber;
            Status = viewModel.Status;
            OptionPassText = viewModel.OptionPassText;
            OptionFailText = viewModel.OptionFailText;
            OptionInProgressText = viewModel.OptionInProgressText;
            Heading = viewModel.Heading;
        }
    }
}
