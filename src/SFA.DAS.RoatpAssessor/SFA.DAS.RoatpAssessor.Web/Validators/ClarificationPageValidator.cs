using Microsoft.AspNetCore.Http;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Clarification;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using SFA.DAS.RoatpAssessor.Web.Helpers;
using SFA.DAS.RoatpAssessor.Web.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Validators
{
    public class ClarificationPageValidator : IClarificationPageValidator
    {
        private const int RequiredMinimumWordsCount = 1;
        private const int MaxWordsCount = 150;
        private const string TooManyWords = "Internal comments must be 150 words or less";
        private const string CommentRequired = "Enter internal comments";

        private const string ClarificationFileRequired = "Upload a file";

        private const int ClarificationResponseMaxWordsCount = 300;
        private const string ClarificationResponseRequired = "Enter clarification response";
        private const string ClarificationResponseTooManyWords = "Clarification response must be 300 words or less";

        private const long MaxFileSizeInBytes = 5 * 1024 * 1024;
        private const string MaxFileSizeExceeded = "The selected file must be smaller than 5MB";
        private const string FileMustBePdf = "The selected file must be a PDF";

        public async Task<ValidationResponse> Validate(SubmitClarificationPageAnswerCommand command)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if(command.ClarificationFileRequired)
            {
                if(!string.IsNullOrWhiteSpace(command.ClarificationFile))
                {
                    // Not validation required as we got a file on record.
                }
                else if (command.FilesToUpload is null || !command.FilesToUpload.Any())
                {
                    validationResponse.Errors.Add(new ValidationErrorDetail("ClarificationFile", ClarificationFileRequired));
                }
            }
            else
            {
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

            if (command.FilesToUpload != null)
            {
                foreach (var file in command.FilesToUpload)
                {
                    if (!FileContentIsValidForPdfFile(file))
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("ClarificationFile", FileMustBePdf));
                        break;
                    }
                    else if(file.Length > MaxFileSizeInBytes)
                    {
                        validationResponse.Errors.Add(new ValidationErrorDetail("ClarificationFile", MaxFileSizeExceeded));
                        break;
                    }
                }
            }

            return await Task.FromResult(validationResponse);
        }

        private static bool FileContentIsValidForPdfFile(IFormFile file)
        {
            var pdfHeader = new byte[] { 0x25, 0x50, 0x44, 0x46 };

            using (var fileContents = file.OpenReadStream())
            {
                var headerOfActualFile = new byte[pdfHeader.Length];
                fileContents.Read(headerOfActualFile, 0, headerOfActualFile.Length);
                fileContents.Position = 0;

                return headerOfActualFile.SequenceEqual(pdfHeader);
            }
        }
    }

    public interface IClarificationPageValidator
    {
        Task<ValidationResponse> Validate(SubmitClarificationPageAnswerCommand command);
    }
}
