using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.UnitTests.MockedObjects;
using System.Security.Claims;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Domain
{
    [TestFixture]
    public class UserExtensionsTests
    {
        private const string GivenName = "Test";
        private const string Surname = "User";
        private const string Email = "Test.User@example.com";

        private ClaimsPrincipal _user;

        [SetUp]
        public void Setup()
        {
            _user = MockedUser.Setup();
        }

        [Test]
        public void UserDisplayName_returns_expected_result()
        {
            var expectedresult = $"{GivenName} {Surname}";

            var actualResult = _user.UserDisplayName();

            Assert.That(actualResult, Is.EqualTo(expectedresult));          
        }

        [Test]
        public void GivenName_returns_expected_result()
        {
            var expectedresult = $"{GivenName}";

            var actualResult = _user.GivenName();

            Assert.That(actualResult, Is.EqualTo(expectedresult));
        }

        [Test]
        public void Surname_returns_expected_result()
        {
            var expectedresult = $"{Surname}";

            var actualResult = _user.Surname();

            Assert.That(actualResult, Is.EqualTo(expectedresult));
        }
    }
}
