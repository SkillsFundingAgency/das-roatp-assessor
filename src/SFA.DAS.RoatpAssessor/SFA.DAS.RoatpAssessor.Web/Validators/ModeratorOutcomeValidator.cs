using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using SFA.DAS.RoatpAssessor.Web.Helpers;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Validators
{
    public class ModeratorOutcomeValidator: IModeratorOutcomeValidator
    {
        private const string StatusRequired = "Select the outcome for this application";
        private const int RequiredMinimumWordsCount = 1;
        private const int MaxWordsCount = 150;
        private const string TooManyWords = "Internal comments must be 150 words or less";
        private const string FailCommentRequired = "Enter internal comments";
        private const string FailTooManyWords = "Internal comments must be 150 words or less";

        private const string EnterAPassConfirmation = "Select if you're sure you want to pass this application";

        public async Task<ValidationResponse> Validate(SubmitModeratorOutcomeCommand command)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrWhiteSpace(command.Status))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.Status), StatusRequired));
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
                    case ModeratorPageReviewStatus.AskForClarification:
                        {
                            var wordCount = ValidationHelper.GetWordCount(command.OptionAskForClarificationText);
                            if (wordCount > MaxWordsCount)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.OptionAskForClarificationText), TooManyWords));
                            }

                            break;
                        }
                }
            }

            return await Task.FromResult(validationResponse);
        }

        public async Task<ValidationResponse> Validate(SubmitModeratorOutcomeConfirmationCommand command)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrEmpty(command.ConfirmStatus))
            {
                if (command.Status == ModerationStatus.Pass)
                    validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.ConfirmStatus), EnterAPassConfirmation));
                else
                {
                    // will be removed once APR-1720 and APR-1721 are complete
                    validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.ConfirmStatus), "Pick a choice"));

                }
            }

            return await Task.FromResult(validationResponse);
        }
    }
    



    public interface IModeratorOutcomeValidator
    {
        Task<ValidationResponse> Validate(SubmitModeratorOutcomeCommand command);
        Task<ValidationResponse> Validate(SubmitModeratorOutcomeConfirmationCommand command);
    }
}
