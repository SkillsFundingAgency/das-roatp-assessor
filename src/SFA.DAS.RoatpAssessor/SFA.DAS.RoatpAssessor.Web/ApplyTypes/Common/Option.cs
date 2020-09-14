using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes.Common
{
    public class Option
    {
        public IEnumerable<Question> FurtherQuestions { get; set; }
        public string Value { get; set; }
        public string Label { get; set; }
        public string HintText { get; set; }
    }
}
