using System.Collections.Generic;
using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Validation;

namespace SFA.DAS.RoatpAssessor.Web.Validators
{
    public class SearchTermValidator : ISearchTermValidator
    {
        private const string SearchTerm = "SearchTerm";
        private const int MinimumSearchTermLength = 3;
        private readonly string EnterSearchTerm = $"Enter {MinimumSearchTermLength} or more characters";

        public ValidationResponse Validate(string searchTerm)
        {
            var validationResponse = new ValidationResponse
            {
                Errors = new List<ValidationErrorDetail>()
            };

            if (string.IsNullOrWhiteSpace(searchTerm) || searchTerm.Length < MinimumSearchTermLength)
                validationResponse.Errors.Add(new ValidationErrorDetail(SearchTerm, EnterSearchTerm));

            return validationResponse;
        }
    }

    public interface ISearchTermValidator
    {
        ValidationResponse Validate(string searchTerm);
    }
}
