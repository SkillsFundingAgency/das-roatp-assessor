using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Validators;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Validators
{
    [TestFixture]
    public class ClarificationPageValidatorTests
    {
        private ClarificationPageValidator _validator;
        private SubmitClarificationPageAnswerCommand _command;

        [SetUp]
        public void SetUp()
        {
            _validator = new ClarificationPageValidator();
            _command = new SubmitClarificationPageAnswerCommand { Heading = "heading", Status = ClarificationPageReviewStatus.Pass, ClarificationResponse = "valid response" };
        }

        [Test]
        public async Task When_status_is_not_provided_then_an_error_is_returned()
        {
            _command.Status = "";

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual($"Select the outcome for {_command.Heading.ToLower()}", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionPass", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_pass_and_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _command.Status = ClarificationPageReviewStatus.Pass;
            _command.OptionPassText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Your comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionPassText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_fail_and_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _command.Status = ClarificationPageReviewStatus.Fail;
            _command.OptionFailText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Internal comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionFailText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_fail_and_word_count_is_below_minimum_then_an_error_is_returned()
        {
            _command.Status = ClarificationPageReviewStatus.Fail;
            _command.OptionFailText = "";

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Enter internal comments", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionFailText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_in_progress_and_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _command.Status = ClarificationPageReviewStatus.InProgress;
            _command.OptionInProgressText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Your comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionInProgressText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_pass_and_word_count_is_below_maximum_then_no_error_is_returned()
        {
            _command.Status = ClarificationPageReviewStatus.Pass;
            _command.OptionPassText = string.Concat(Enumerable.Repeat("test ", 150));

            var response = await _validator.Validate(_command);

            Assert.IsTrue(response.IsValid);
        }

        [Test]
        public async Task When_status_is_in_progress_and_word_count_is_below_maximum_then_no_error_is_returned()
        {
            _command.Status = ClarificationPageReviewStatus.InProgress;
            _command.OptionInProgressText = string.Concat(Enumerable.Repeat("test ", 150));

            var response = await _validator.Validate(_command);

            Assert.IsTrue(response.IsValid);
        }

        [Test]
        public async Task When_clarification_response_is_not_provided_then_an_error_is_returned()
        {
            _command.ClarificationResponse = "";

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Enter clarification response", response.Errors.First().ErrorMessage);
            Assert.AreEqual("ClarificationResponse", response.Errors.First().Field);
        }

        [Test]
        public async Task When_clarification_response_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _command.ClarificationResponse = string.Concat(Enumerable.Repeat("test ", 301));

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Clarification response must be 300 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("ClarificationResponse", response.Errors.First().Field);
        }
    }
}
