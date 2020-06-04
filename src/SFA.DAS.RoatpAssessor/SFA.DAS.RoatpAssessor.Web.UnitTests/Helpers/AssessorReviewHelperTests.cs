using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.Helpers;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Helpers
{
    [TestFixture]
    public class AssessorReviewHelperTests
    {
        private const string _Assessor1UserId = "User1";
        private const string _Assessor2UserId = "User2";

        private readonly Apply _application = new Apply { Assessor1UserId = _Assessor1UserId, Assessor2UserId = _Assessor2UserId };

        [TestCase(null, AssessorType.Undefined)]
        [TestCase("", AssessorType.Undefined)]
        [TestCase(" ", AssessorType.Undefined)]
        [TestCase("\t", AssessorType.Undefined)]
        [TestCase(_Assessor1UserId, AssessorType.FirstAssessor)]
        [TestCase(_Assessor2UserId, AssessorType.SecondAssessor)]
        [TestCase("INVALID", AssessorType.Undefined)]
        public void SetAssessorType_returns_expected_result(string userId, AssessorType expectedAssessorType)
        {
            var result = AssessorReviewHelper.SetAssessorType(_application, userId);

            Assert.That(result, Is.EqualTo(expectedAssessorType));
        }
    }
}

