using SFA.DAS.RoatpAssessor.Web.ViewModels;

namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class AddUserDetailsCommand
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public AddUserDetailsCommand()
        {
            // deliberately left it empty. 
        }

        public AddUserDetailsCommand(AddUserDetailsModel viewModel)
        {
            FirstName = viewModel.FirstName;
            LastName = viewModel.LastName;
        }
    }
}
