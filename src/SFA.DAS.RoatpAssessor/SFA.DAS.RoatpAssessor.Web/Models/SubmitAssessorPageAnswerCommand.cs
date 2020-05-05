using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class SubmitAssessorPageAnswerCommand
    {
        public Guid ApplicationId { get; set; }
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }
        public string Status { get; set; }
        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }
        public string OptionInProgressText { get; set; }
        public AssessorType AssessorType { get; set; }

        public SubmitAssessorPageAnswerCommand()
        {
                
        }

        public SubmitAssessorPageAnswerCommand(ReviewAnswersViewModel viewModel)
        {
            ApplicationId = viewModel.ApplicationId;
            PageId = viewModel.PageId;
            SequenceNumber = viewModel.SequenceNumber;
            SectionNumber = viewModel.SectionNumber;
            Status = viewModel.Status;
            OptionPassText = viewModel.OptionPassText;
            OptionFailText = viewModel.OptionFailText;
            OptionInProgressText = viewModel.OptionInProgressText;
            AssessorType = viewModel.AssessorType;
        }
    }
}
