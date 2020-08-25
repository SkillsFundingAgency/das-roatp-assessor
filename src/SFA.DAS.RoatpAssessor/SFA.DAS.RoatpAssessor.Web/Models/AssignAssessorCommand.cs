namespace SFA.DAS.RoatpAssessor.Web.Models
{
    public class AssignAssessorCommand
    {
        public AssignAssessorCommand(int assessorNumber, string assessorUserId, string assessorName)
        {
            AssessorNumber = assessorNumber;
            AssessorUserId = assessorUserId;
            AssessorName = assessorName;
        }

        public int AssessorNumber { get; }
        public string AssessorUserId { get; }
        public string AssessorName { get; }
    }
}
