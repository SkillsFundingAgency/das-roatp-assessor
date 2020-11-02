using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Validators;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Validators
{
    [TestFixture]
    public class ClarificationOutcomeValidatorTests
    {
        private ClarificationOutcomeValidator _validator;
        private SubmitClarificationOutcomeCommand _submitClarificationOutcomeCommand;
        private SubmitClarificationOutcomeConfirmationCommand _outcomeConfirmationCommand;


        [SetUp]
        public void SetUp()
        {
            _validator = new ClarificationOutcomeValidator();
            _submitClarificationOutcomeCommand = new SubmitClarificationOutcomeCommand();
            _outcomeConfirmationCommand = new SubmitClarificationOutcomeConfirmationCommand();
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

        [Test]
        public async Task When_confirm_status_is_not_provided_and_status_is_pass_then_an_error_is_returned()
        {
            _outcomeConfirmationCommand.ConfirmStatus = "";
            _outcomeConfirmationCommand.Status = ModerationStatus.Pass;
            var response = await _validator.Validate(_outcomeConfirmationCommand);
            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Select if you're sure you want to pass this application", response.Errors.First().ErrorMessage);
            Assert.AreEqual("ConfirmStatus", response.Errors.First().Field);
        }

        [Test]
        public async Task When_confirm_status_is_not_provided_and_status_not_provided_then_an_error_is_returned()
        {
            _outcomeConfirmationCommand.ConfirmStatus = "";

            var response = await _validator.Validate(_outcomeConfirmationCommand);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Pick a choice", response.Errors.First().ErrorMessage);
            Assert.AreEqual("ConfirmStatus", response.Errors.First().Field);
        }

        [Test]
        public async Task When_confirm_status_is_not_provided_and_status_is_fail_then_an_error_is_returned()
        {
            _outcomeConfirmationCommand.ConfirmStatus = "";
            _outcomeConfirmationCommand.Status = ClarificationConfirmationStatus.Fail;
            var response = await _validator.Validate(_outcomeConfirmationCommand);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Select if you're sure you want to fail this application", response.Errors.First().ErrorMessage);
            Assert.AreEqual("ConfirmStatus", response.Errors.First().Field);
        }

        [Test]
        public async Task When_confirm_status_is_provided_then_no_error_is_returned()
        {
            _outcomeConfirmationCommand.ConfirmStatus = "anything";

            var response = await _validator.Validate(_outcomeConfirmationCommand);

            Assert.IsTrue(response.IsValid);
        }
    }
}
