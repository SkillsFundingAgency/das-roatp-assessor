using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Validators;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Validators
{
    [TestFixture]
    public class RoatpAssessorOutcomeValidatorTests
    {
        private RoatpAssessorOutcomeValidator _validator;
        private SubmitAssessorOutcomeCommand _command;

        [SetUp]
        public void SetUp()
        {
            _validator = new RoatpAssessorOutcomeValidator();
            _command = new SubmitAssessorOutcomeCommand();
        }

        [Test]
        public async Task When_MoveToModeration_is_not_provided_then_an_error_is_returned()
        {
            _command.MoveToModeration = "";

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual($"Select if you're sure this application is ready for moderation", response.Errors.First().ErrorMessage);
            Assert.AreEqual("MoveToModeration", response.Errors.First().Field);
        }

        [Test]
        public async Task When_MoveToModeration_is_invalid_value_then_an_error_is_returned()
        {
            _command.MoveToModeration = "INVALID";

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual($"Unknown option selected", response.Errors.First().ErrorMessage);
            Assert.AreEqual("MoveToModeration", response.Errors.First().Field);
        }

        [Test]
        public async Task When_MoveToModeration_is_YES_then_no_error_is_returned()
        {
            _command.MoveToModeration = "YES";

            var response = await _validator.Validate(_command);

            Assert.IsTrue(response.IsValid);
        }

        [Test]
        public async Task When_MoveToModeration_is_NO_then_no_error_is_returned()
        {
            _command.MoveToModeration = "NO";

            var response = await _validator.Validate(_command);

            Assert.IsTrue(response.IsValid);
        }
    }
}
