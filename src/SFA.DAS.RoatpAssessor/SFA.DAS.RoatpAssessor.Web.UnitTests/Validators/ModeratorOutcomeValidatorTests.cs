﻿using NUnit.Framework;
using SFA.DAS.RoatpAssessor.Web.Models;
using SFA.DAS.RoatpAssessor.Web.Validators;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;

namespace SFA.DAS.RoatpAssessor.Web.UnitTests.Validators
{
    [TestFixture]
    public class ModeratorOutcomeValidatorTests
    {
        private ModeratorOutcomeValidator _validator;
        private SubmitModeratorOutcomeCommand _command;

        [SetUp]
        public void SetUp()
        {
            _validator = new ModeratorOutcomeValidator();
            _command = new SubmitModeratorOutcomeCommand();
        }

        [Test]
        public async Task When_status_is_not_provided_then_an_error_is_returned()
        {
            _command.Status = "";

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Select the outcome for this application", response.Errors.First().ErrorMessage);
            Assert.AreEqual("Status", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_pass_and_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.Pass;
            _command.OptionPassText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Internal comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionPassText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_fail_and_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.Fail;
            _command.OptionFailText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Internal comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionFailText", response.Errors.First().Field);
        }


        [Test]
        public async Task When_status_is_fail_and_word_count_is_below_minimum_then_an_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.Fail;
            _command.OptionFailText = "";

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Enter internal comments", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionFailText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_ask_for_clarification_and_word_count_exceeds_maximum_then_an_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.AskForClarification;
            _command.OptionAskForClarificationText = string.Concat(Enumerable.Repeat("test ", 151));

            var response = await _validator.Validate(_command);

            Assert.IsFalse(response.IsValid);
            Assert.AreEqual("Internal comments must be 150 words or less", response.Errors.First().ErrorMessage);
            Assert.AreEqual("OptionAskForClarificationText", response.Errors.First().Field);
        }

        [Test]
        public async Task When_status_is_pass_and_word_count_is_empty_then_no_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.Pass;
            _command.OptionAskForClarificationText = "";

            var response = await _validator.Validate(_command);

            Assert.IsTrue(response.IsValid);
        }

        [Test]
        public async Task When_status_is_ask_for_clarification_and_word_count_is_below_maximum_then_no_error_is_returned()
        {
            _command.Status = ModeratorPageReviewStatus.InProgress;
            _command.OptionAskForClarificationText = string.Concat(Enumerable.Repeat("test ", 150));

            var response = await _validator.Validate(_command);

            Assert.IsTrue(response.IsValid);
        }
    }
}
