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
            // TODO: We shouldn't need this function in Assessor. The back end service should determine which assessor type it is
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
