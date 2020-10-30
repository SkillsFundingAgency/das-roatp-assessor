using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Apply;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Moderator;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using SFA.DAS.RoatpAssessor.Web.Helpers;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Validators
    {
        public class ClarificationOutcomeValidator : IClarificationOutcomeValidator
    {
        //MFCMFC all this must be checked/changed
        // not sure inprogress is needed here
            private const string StatusRequired = "Select the outcome for this application";
            private const int RequiredMinimumWordsCount = 1;
            private const int MaxWordsCount = 150;
            private const string TooManyWords = "Internal comments must be 150 words or less";
            private const string FailCommentRequired = "Enter internal comments";
            private const string FailTooManyWords = "Internal comments must be 150 words or less";

            public async Task<ValidationResponse> Validate(SubmitClarificationOutcomeCommand command)
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
                                    validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.OptionFailText), FailCommentRequired));
                                }
                                else if (wordCount > MaxWordsCount)
                                {
                                    validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.OptionFailText), FailTooManyWords));
                                }

                                break;
                            }
                    }
                }

                return await Task.FromResult(validationResponse);
            }

        }

        public interface IClarificationOutcomeValidator
        {
            Task<ValidationResponse> Validate(SubmitClarificationOutcomeCommand command);
        }
    }
