using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using SFA.DAS.RoatpAssessor.Web.Helpers;
using SFA.DAS.RoatpAssessor.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Validators
{
    public class ClarificationPageValidator : IClarificationPageValidator
    {
        private const int RequiredMinimumWordsCount = 1;
        private const int MaxWordsCount = 150;
        private const int ClarificationResponseMaxWordsCount = 300;
        private const string ClarificationResponseRequired = "Enter clarification response";
        private const string ClarificationResponseTooManyWords = "Clarification response must be 300 words or less";

        private const string TooManyWords = "Internal comments must be 150 words or less";
        private const string CommentRequired = "Enter internal comments";


        public async Task<ValidationResponse> Validate(SubmitClarificationPageAnswerCommand command)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrWhiteSpace(command.ClarificationResponse))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.ClarificationResponse), ClarificationResponseRequired));
            }
            else
            {
                var wordCount = ValidationHelper.GetWordCount(command.ClarificationResponse);
                if (wordCount > ClarificationResponseMaxWordsCount)
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.ClarificationResponse), ClarificationResponseTooManyWords));
                }
            }

            if (string.IsNullOrWhiteSpace(command.Status))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail("OptionPass", ValidationHelper.StatusMandatoryValidationMessage(command.PageId, command.Heading)));
            }
            else
            {
                switch (command.Status)
                {
                    case ClarificationPageReviewStatus.Pass:
                        {
                            var wordCount = ValidationHelper.GetWordCount(command.OptionPassText);
                            if (wordCount > MaxWordsCount)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.OptionPassText), TooManyWords));
                            }

                            break;
                        }
                    case ClarificationPageReviewStatus.Fail:
                        {
                            var wordCount = ValidationHelper.GetWordCount(command.OptionFailText);
                            if (wordCount < RequiredMinimumWordsCount)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.OptionFailText), CommentRequired));
                            }
                            else if (wordCount > MaxWordsCount)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.OptionFailText), TooManyWords));
                            }

                            break;
                        }
                    case ClarificationPageReviewStatus.InProgress:
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

    public interface IClarificationPageValidator
    {
        Task<ValidationResponse> Validate(SubmitClarificationPageAnswerCommand command);
    }
}
