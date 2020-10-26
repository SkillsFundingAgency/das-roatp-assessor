using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Validators;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Validators
{
    [TestFixture]
    public class AssessorPageValidatorTests
    {
        private AssessorPageValidator _validator;
        private SubmitAssessorPageAnswerCommand _command;

        [SetUp]
        public void SetUp()
        {
            _validator = new AssessorPageValidator();
            _command = new SubmitAssessorPageAnswerCommand { Heading = "heading" };
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
            _command.Status = AssessorPageReviewStatus.Pass;
            _command.OptionPassText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Your comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionPassText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_fail_and_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _command.Status = AssessorPageReviewStatus.Fail;
            _command.OptionFailText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Your comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionFailText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_fail_and_word_count_is_below_minimum_then_an_error_is_returned()
        {
            _command.Status = AssessorPageReviewStatus.Fail;
            _command.OptionFailText = "";

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Enter comments", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionFailText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_in_progress_and_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _command.Status = AssessorPageReviewStatus.InProgress;
            _command.OptionInProgressText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Your comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionInProgressText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_pass_and_word_count_is_beiow_maximum_then_no_error_is_returned()
        {
            _command.Status = AssessorPageReviewStatus.Pass;
            _command.OptionPassText = string.Concat(Enumerable.Repeat("test ", 150));

            var response = await _validator.Validate(_command);

            Assert.IsTrue(response.IsValid);
        }

        [Test]
        public async Task When_status_is_fail_and_word_count_is_beiow_maximum_then_no_error_is_returned()
        {
            _command.Status = AssessorPageReviewStatus.Fail;
            _command.OptionFailText = string.Concat(Enumerable.Repeat("test ", 150));

            var response = await _validator.Validate(_command);

            Assert.IsTrue(response.IsValid);
        }

        [Test]
        public async Task When_status_is_in_progress_and_word_count_is_beiow_maximum_then_no_error_is_returned()
        {
            _command.Status = AssessorPageReviewStatus.InProgress;
            _command.OptionInProgressText = string.Concat(Enumerable.Repeat("test ", 150));

            var response = await _validator.Validate(_command);

            Assert.IsTrue(response.IsValid);
        }
    }
}
