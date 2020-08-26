using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Validators;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Validators
{
    [TestFixture]
    public class ModeratorPageValidatorTests
    {
        private ModeratorPageValidator _validator;
        private SubmitModeratorPageAnswerCommand _command;

        [SetUp]
        public void SetUp()
        {
            _validator = new ModeratorPageValidator();
            _command = new SubmitModeratorPageAnswerCommand { Heading = "heading" };
        }

        [Test]
        public async Task When_status_is_not_provided_then_an_error_is_returned()
        {
            _command.Status = "";

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual($"Select the outcome for {_command.Heading.ToLower()}", response.Errors.First().ErrorMessage);
            Assert.AreEqual("Status", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_pass_and_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.Pass;
            _command.OptionPassText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Your comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionPassText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_fail_and_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.Fail;
            _command.OptionFailText = string.Concat(Enumerable.Repeat("test ", 151));
            _command.OptionFailExternalText = "external comment";

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Your comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionFailText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_fail_and_word_count_is_below_minimum_then_an_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.Fail;
            _command.OptionFailText = "";
            _command.OptionFailExternalText = "external comment";

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Enter comments", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionFailText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_fail_and_external_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.Fail;
            _command.OptionFailText = "internal comment";
            _command.OptionFailExternalText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Your comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionFailExternalText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_fail_and_external_word_count_is_below_minimum_then_an_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.Fail;
            _command.OptionFailText = "internal comment";
            _command.OptionFailExternalText = "";

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Enter comments", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionFailExternalText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_in_progress_and_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.InProgress;
            _command.OptionInProgressText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Your comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionInProgressText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_pass_and_word_count_is_below_maximum_then_no_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.Pass;
            _command.OptionPassText = string.Concat(Enumerable.Repeat("test ", 150));

            var response = await _validator.Validate(_command);

            Assert.IsTrue(response.IsValid);
        }

        [Test]
        public async Task When_status_is_fail_and_both_word_counts_are_below_maximum_then_no_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.Fail;
            _command.OptionFailText = string.Concat(Enumerable.Repeat("test ", 150));
            _command.OptionFailExternalText = string.Concat(Enumerable.Repeat("test ", 150));

            var response = await _validator.Validate(_command);

            Assert.IsTrue(response.IsValid);
        }

        [Test]
        public async Task When_status_is_in_progress_and_word_count_is_below_maximum_then_no_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.InProgress;
            _command.OptionInProgressText = string.Concat(Enumerable.Repeat("test ", 150));

            var response = await _validator.Validate(_command);

            Assert.IsTrue(response.IsValid);
        }
    }
}
