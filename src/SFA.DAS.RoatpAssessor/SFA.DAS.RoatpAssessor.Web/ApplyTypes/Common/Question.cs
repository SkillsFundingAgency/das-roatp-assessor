using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common
{
    public class Question
    {
        public string QuestionId { get; set; }
        public string Label { get; set; }
        public string QuestionBodyText { get; set; }
        public string InputType { get; set; }
        public string InputPrefix { get; set; }
        public string InputSuffix { get; set; }

        public IEnumerable<Option> Options { get; set; }
    }
}
