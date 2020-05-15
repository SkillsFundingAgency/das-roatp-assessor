using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Helpers
{
    public static class AssessorReviewHelpers
    {
        public static AssessorType SetAssessorType(Apply application, string userId)
        {
            if (userId.Equals(application.Assessor1UserId))
            {
                return AssessorType.FirstAssessor;
            }
            else if (userId.Equals(application.Assessor2UserId))
            {
                return AssessorType.SecondAssessor;
            }
            else
            {
                return AssessorType.Undefined;
            }
        }
    }
}
