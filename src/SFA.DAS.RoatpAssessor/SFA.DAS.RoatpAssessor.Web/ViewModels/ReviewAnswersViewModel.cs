using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class ReviewAnswersViewModel
    {
        public Guid ApplicationId { get; set; }

        public string Status { get; set; }

        public string Heading { get; set; }
        public string Caption { get; set; }

        public string OptionPassText { get; set; }
        public string OptionFailText { get; set; }
        public string OptionInProgressText { get; set; }
        public List<ValidationErrorDetail> ErrorMessages { get; set; }

        // Will not need them. Just for testing
        public int SequenceNumber { get; set; }
        public int SectionNumber { get; set; }
    }


    public class ValidationErrorDetail
    {
        public ValidationErrorDetail()
        {
        }

        public ValidationErrorDetail(string field, string errorMessage)
        {
            Field = field;
            ErrorMessage = errorMessage;
        }

        public ValidationErrorDetail(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public string Field { get; set; }
        public string ErrorMessage { get; set; }
    }
}
