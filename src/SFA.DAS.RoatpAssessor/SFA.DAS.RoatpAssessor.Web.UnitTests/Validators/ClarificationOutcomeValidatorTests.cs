using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Validators;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Validators
{
    [TestFixture]
    public class ClarificationOutcomeValidatorTests
    {
        private ClarificationOutcomeValidator _validator;
        private SubmitClarificationOutcomeCommand _submitClarificationOutcomeCommand;


        [SetUp]
        public void SetUp()
        {
            _validator = new ClarificationOutcomeValidator();
            _submitClarificationOutcomeCommand = new SubmitClarificationOutcomeCommand();
        }

        [Test]
        public async Task When_status_is_not_provided_then_an_error_is_returned()
        {
            _submitClarificationOutcomeCommand.Status = "";

            var response = await _validator.Validate(_submitClarificationOutcomeCommand);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Select the outcome for this application", response.Errors.First().ErrorMessage);
            Assert.AreEqual("Status", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_pass_and_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _submitClarificationOutcomeCommand.Status = ClarificationPageReviewStatus.Pass;
            _submitClarificationOutcomeCommand.OptionPassText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_submitClarificationOutcomeCommand);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Internal comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionPassText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_fail_and_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _submitClarificationOutcomeCommand.Status = ClarificationPageReviewStatus.Fail;
            _submitClarificationOutcomeCommand.OptionFailText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_submitClarificationOutcomeCommand);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Internal comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionFailText", response.Errors.First().Field);
        }


        [Test]
        public async Task When_status_is_fail_and_word_count_is_below_minimum_then_an_error_is_returned()
        {
            _submitClarificationOutcomeCommand.Status = ClarificationPageReviewStatus.Fail;
            _submitClarificationOutcomeCommand.OptionFailText = "";

            var response = await _validator.Validate(_submitClarificationOutcomeCommand);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Enter internal comments", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionFailText", response.Errors.First().Field);
        }


        [Test]
        public async Task When_status_is_pass_and_word_count_is_empty_then_no_error_is_returned()
        {
            _submitClarificationOutcomeCommand.Status = ClarificationPageReviewStatus.Pass;
            _submitClarificationOutcomeCommand.OptionPassText = "";

            var response = await _validator.Validate(_submitClarificationOutcomeCommand);

            Assert.IsTrue(response.IsValid);
        }
    }
}
