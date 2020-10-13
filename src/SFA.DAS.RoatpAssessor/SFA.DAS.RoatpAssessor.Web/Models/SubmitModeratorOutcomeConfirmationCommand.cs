namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class SubmitModeratorOutcomeConfirmationCommand
    {
        public string Status { get; set; }
        public string ConfirmStatus { get; set; }


        public SubmitModeratorOutcomeConfirmationCommand()
        {
        }

        public SubmitModeratorOutcomeConfirmationCommand(string status, string confirmStatus)
        {
            Status = status;
            ConfirmStatus = confirmStatus;
        }
    }
}