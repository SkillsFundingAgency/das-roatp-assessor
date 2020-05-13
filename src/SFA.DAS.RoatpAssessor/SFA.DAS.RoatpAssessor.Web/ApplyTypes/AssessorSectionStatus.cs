using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.ApplyTypes
{
    public class AssessorSectionStatus
    {
        public const string InProgress = "In progress";
        public const string Pass = "Pass";
        public const string Fail = "Fail";
        public const string FailOutOf = "{0} FAIL OUT OF {1}";
    }
}
