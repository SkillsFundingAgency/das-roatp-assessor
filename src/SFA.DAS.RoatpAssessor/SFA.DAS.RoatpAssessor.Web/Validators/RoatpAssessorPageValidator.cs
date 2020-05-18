using SFA.DAS.RoatpAssessor.Web.ApplyTypes;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using SFA.DAS.RoatpAssessor.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Validators
{
    public class RoatpAssessorPageValidator : IRoatpAssessorPageValidator
    {
        private const int MaxWordsAcount = 150;
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
                validationResponse.Errors.Add(new ValidationErrorDetail("OptionPass", $"Select the outcome for {command.Heading.ToLower()}"));
            }
            else
            {
                if (command.Status == AssessorPageReviewStatus.Fail && string.IsNullOrEmpty(command.OptionFailText))
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                        FailDetailsRequired));
                }
            }

            if (validationResponse.Errors.Any())
            {
                return await Task.FromResult(validationResponse);
            }

            switch (command.Status)
            {
                case AssessorPageReviewStatus.Pass when !string.IsNullOrEmpty(command.OptionPassText):
                    {
                        var wordCount = command.OptionPassText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                        if (wordCount > MaxWordsAcount)
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionPassText",
                                TooManyWords));
                        }

                        break;
                    }
                case AssessorPageReviewStatus.Fail when !string.IsNullOrEmpty(command.OptionFailText):
                    {
                        var wordCount = command.OptionFailText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).Length;
                        if (wordCount > MaxWordsAcount)
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionFailText",
                                TooManyWords));
                        }

                        break;
                    }
                case AssessorPageReviewStatus.InProgress when !string.IsNullOrEmpty(command.OptionInProgressText):
                    {
                        var wordCount = command.OptionInProgressText.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                            .Length;
                        if (wordCount > MaxWordsAcount)
                        {
                            validationResponse.Errors.Add(new ValidationErrorDetail("OptionInProgressText",
                                TooManyWords));
                        }

                        break;
                    }
            }

            return await Task.FromResult(validationResponse);
        }
    }

    public interface IRoatpAssessorPageValidator
    {
        Task<ValidationResponse> Validate(SubmitAssessorPageAnswerCommand command);
    }
}
