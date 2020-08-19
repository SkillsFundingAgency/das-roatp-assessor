using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;

namespace SFA.DAS.RoatpAssessor.Web.Helpers
{
    public static class AssessorReviewHelper
    {
        public static AssessorType SetAssessorType(Apply application, string userId)
        {
            // TODO: We shouldn't need this function in Assessor. The back end service should determine which assessor type it is
            if (string.IsNullOrEmpty(userId))
            {
                return AssessorType.Undefined;
            }
            else if (userId.Equals(application.Assessor1UserId))
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
