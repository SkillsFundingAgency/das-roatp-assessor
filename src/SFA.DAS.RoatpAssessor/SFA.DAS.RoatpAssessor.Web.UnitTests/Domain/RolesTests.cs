using System;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Domain;
using SFA.DAS.RoatpAssessor.Web.UnitTests.Extensions;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Domain
{
    [TestFixture]
    public class RolesTests
    {
        [Test]
        public void When_invalid_role_HasValidRole_returns_False()
        {
            var knownInvalidRole = Guid.Empty.ToString("n");
            var user = ControllerExtensions.GetMockedUser(knownInvalidRole);

            var actualResult = Roles.HasValidRole(user);

            Assert.That(actualResult, Is.False);
        }

        [Test]
        public void When_valid_role__HasValidRole_returns_True()
        {
            var knownValidRole = Roles.RoatpAssessorTeam;
            var user = ControllerExtensions.GetMockedUser(knownValidRole);

            var actualResult = Roles.HasValidRole(user);

            Assert.That(actualResult, Is.True);
        }
    }
}
