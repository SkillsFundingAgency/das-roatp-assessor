using System.Collections.Generic;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common;

namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class FurtherQuestionsViewModel
    {
        public IEnumerable<Option> Options { get; set; }
        public List<Answer> Answers { get; set; }
    }
}