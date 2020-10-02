using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using SFA.DAS.RoatpAssessor.Web.Helpers;
using SFA.DAS.RoatpAssessor.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Validators
{
    public class ModeratorPageValidator : IModeratorPageValidator
    {
        private const int RequiredMinimumWordsCount = 1;
        private const int MaxWordsCount = 150;
        private const string TooManyWords = "Your comments must be 150 words or less";
        private const string FailCommentRequired = "Enter internal comments";
        private const string FailTooManyWords = "Internal comments must be 150 words or less";

        public async Task<ValidationResponse> Validate(SubmitModeratorPageAnswerCommand command)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrWhiteSpace(command.Status))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.Status), ValidationHelper.StatusMandatoryValidationMessage(command.PageId, command.Heading)));
            }
            else
            {
                switch (command.Status)
                {
                    case ModeratorPageReviewStatus.Pass:
                        {
                            var wordCount = ValidationHelper.GetWordCount(command.OptionPassText);
                            if (wordCount > MaxWordsCount)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.OptionPassText), TooManyWords));
                            }

                            break;
                        }
                    case ModeratorPageReviewStatus.Fail:
                        {
                            var wordCount = ValidationHelper.GetWordCount(command.OptionFailText);
                            if (wordCount < RequiredMinimumWordsCount)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.OptionFailText), FailCommentRequired));
                            }
                            else if (wordCount > MaxWordsCount)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.OptionFailText), FailTooManyWords));
                            }

                            break;
                        }
                    case ModeratorPageReviewStatus.InProgress:
                        {
                            var wordCount = ValidationHelper.GetWordCount(command.OptionInProgressText);
                            if (wordCount > MaxWordsCount)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.OptionInProgressText), TooManyWords));
                            }

                            break;
                        }
                }
            }

            return await Task.FromResult(validationResponse);
        }
    }

    public interface IModeratorPageValidator
    {
        Task<ValidationResponse> Validate(SubmitModeratorPageAnswerCommand command);
    }
}
