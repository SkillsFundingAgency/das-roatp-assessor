using Microsoft.AspNetCore.Http;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ViewModels;
using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class SubmitClarificationPageAnswerCommand
    {
        public Guid ApplicationId { get; set; }
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
        public string PageId { get; set; }
        public string NextPageId { get; set; }
        public string Status { get; set; }
        public bool ClarificationRequired { get; set; }
        public string ClarificationResponse { get; set; }
        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }
        public string OptionInProgressText { get; set; }
        public string Heading { get; set; }

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

        public IFormFileCollection FilesToUpload { get; set; }

        public SubmitClarificationPageAnswerCommand()
        {
                
        }

        public SubmitClarificationPageAnswerCommand(ClarifierReviewAnswersViewModel viewModel)
        {
            ApplicationId = viewModel.ApplicationId;
            PageId = viewModel.PageId;
            NextPageId = viewModel.NextPageId;
            SequenceNumber = viewModel.SequenceNumber;
            SectionNumber = viewModel.SectionNumber;
            Status = viewModel.Status;
            ClarificationRequired = viewModel.ClarificationRequired;
            ClarificationResponse = viewModel.ClarificationResponse;
            OptionPassText = viewModel.OptionPassText;
            OptionFailText = viewModel.OptionFailText;
            OptionInProgressText = viewModel.OptionInProgressText;
            Heading = viewModel.Heading;
        }
    }
}
