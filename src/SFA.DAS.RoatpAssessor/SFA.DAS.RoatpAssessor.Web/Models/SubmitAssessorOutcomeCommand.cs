using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using System;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class SubmitAssessorOutcomeCommand
    {
        public Guid ApplicationId { get; set; }
        public AssessorType AssessorType { get; set; }
        public string MoveToModeration { get; set; }
    }
}
