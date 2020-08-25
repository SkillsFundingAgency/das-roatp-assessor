using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Assessor;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using SFA.DAS.RoatpAssessor.Web.Helpers;
using SFA.DAS.RoatpAssessor.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Validators
{
    public class RoatpAssessorPageValidator : IRoatpAssessorPageValidator
    {
        private const int RequiredMinimumWordsCount = 1;
        private const int MaxWordsCount = 150;
        private const string FailDetailsRequired = "Enter comments";
        private const string TooManyWords = "Your comments must be 150 words or less";

        public async Task<ValidationResponse> Validate(SubmitAssessorPageAnswerCommand command)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrWhiteSpace(command.Status))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.Status), ValidationHelper.MandatoryValidationMessage(command)));
            }
            else
            {

                switch (command.Status)
                {
                    case AssessorPageReviewStatus.Pass:
                        {
                            var wordCount = GetWordCount(command.OptionPassText);
                            if (wordCount > MaxWordsCount)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.OptionPassText), TooManyWords));
                            }

                            break;
                        }
                    case AssessorPageReviewStatus.Fail:
                        {
                            var wordCount = GetWordCount(command.OptionFailText);
                            if (wordCount < RequiredMinimumWordsCount)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.OptionFailText), FailDetailsRequired));
                            }
                            else if (wordCount > MaxWordsCount)
                            {
                                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.OptionFailText), TooManyWords));
                            }

                            break;
                        }
                    case AssessorPageReviewStatus.InProgress:
                        {
                            var wordCount = GetWordCount(command.OptionInProgressText);
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

        private static int GetWordCount(string text)
        {
            int wordCount = 0;

            if (!string.IsNullOrWhiteSpace(text))
            {
                wordCount = text.Split(new[] { " ", "\t" }, StringSplitOptions.RemoveEmptyEntries)
                            .Length;
            }

            return wordCount;
        }
    }

    public interface IRoatpAssessorPageValidator
    {
        Task<ValidationResponse> Validate(SubmitAssessorPageAnswerCommand command);
    }
}
