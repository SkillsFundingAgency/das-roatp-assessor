using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using SFA.DAS.RoatpAssessor.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Web.Validators
{
    public class AssessorOutcomeValidator : IAssessorOutcomeValidator
    {
        public Task<ValidationResponse> Validate(SubmitAssessorOutcomeCommand command)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrWhiteSpace(command.MoveToModeration))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.MoveToModeration), $"Select if you're sure this application is ready for moderation"));
            }
            else
            {
                switch (command.MoveToModeration)
                {
                    case "YES":
                    case "NO":
                        break;
                    default:
                        validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.MoveToModeration), $"Unknown option selected"));
                        break;
                }
            }

            return Task.FromResult(validationResponse);
        }
    }

    public interface IAssessorOutcomeValidator
    {
        Task<ValidationResponse> Validate(SubmitAssessorOutcomeCommand command);
    }
}
