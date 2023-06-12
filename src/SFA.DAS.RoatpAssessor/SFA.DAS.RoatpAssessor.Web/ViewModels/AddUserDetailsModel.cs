namespace SFA.DAS.RoatpAssessor.Web.ViewModels
{
    public class AddUserDetailsModel : ViewModelBase
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FirstNameError => GetErrorMessage(nameof(FirstName));
        public string LastNameError => GetErrorMessage(nameof(LastName));
    }
}
