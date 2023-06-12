using SFA.DAS.RoatpAssessor.Web.Models;
using System.Threading.Tasks;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;
using System.Collections.Generic;

namespace SFA.DAS.RoatpAssessor.Web.Validators
{
    public class AddUserDetailsValidator : IAddUserDetailsValidator
    {
        public async Task<ValidationResponse> Validate(AddUserDetailsCommand command)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrWhiteSpace(command.FirstName))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.FirstName), "Enter your first name"));
            }

            if (string.IsNullOrWhiteSpace(command.LastName))
            {
                validationResponse.Errors.Add(new ValidationErrorDetail(nameof(command.LastName), "Enter your last name"));
            }

            return await Task.FromResult(validationResponse);
        }
    }

    public interface IAddUserDetailsValidator
    {
        Task<ValidationResponse> Validate(AddUserDetailsCommand command);
    }
}
