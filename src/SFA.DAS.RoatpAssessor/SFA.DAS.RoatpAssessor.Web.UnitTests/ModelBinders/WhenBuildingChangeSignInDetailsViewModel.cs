using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.ModelBinders
{
    public class WhenBuildingChangeSignInDetailsViewModel
    {
        [Test]
        public void Then_The_Settings_Link_Is_Correct_For_Production_Environment()
        {
            var actual = new ChangeSignInDetailsViewModel("prd");

            Assert.That(actual.SettingsLink, Is.Not.Null);
            Assert.AreEqual(actual.SettingsLink, "https://home.account.gov.uk/settings");
        }
        [Test]
        public void Then_The_Settings_Link_Is_Correct_For_Non_Production_Environment()
        {
            var actual = new ChangeSignInDetailsViewModel("test");

            Assert.That(actual.SettingsLink, Is.Not.Null);
            Assert.AreEqual(actual.SettingsLink, "https://home.integration.account.gov.uk/settings");
        }
    }
}
