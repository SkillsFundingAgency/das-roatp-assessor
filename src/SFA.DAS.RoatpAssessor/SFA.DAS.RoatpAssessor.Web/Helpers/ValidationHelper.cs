using SFA.DAS.RoatpAssessor.Web.ApplyTypes.Consts;
using SFA.DAS.RoatpAssessor.Web.Models;

namespace SFA.DAS.RoatpAssessor.Web.Helpers
{
    public static class ValidationHelper
    {
        public static string MandatoryValidationMessage(SubmitAssessorPageAnswerCommand command)
        {
            switch (command.PageId)
            {
                case RoatpWorkflowPageIds.HowTeamWorkedWithOtherOrganisationsToDevelopAndDeliverTraining:
                    {
                        return "Select the outcome for worked with other organisations to develop and deliver training";
                    }
                case RoatpWorkflowPageIds.HowWillYourOrganisationEngageWithEndPointAssessmentOrganisations:
                    {
                        return "Select the outcome for engaging with end-point assessment organisations (EPAO's)";
                    }
                case RoatpWorkflowPageIds.HowPersonWorkedWithEmployersToDevelopAndDeliverTraining:
                    {
                        return "Select the outcome for worked with employers to develop and deliver training";
                    }
                case RoatpWorkflowPageIds.OrganisationHasResourcesToSubmitILRData:
                    {
                        return "Select the outcome for Individualised Learner Record (ILR) data";
                    }
                default:
                    {
                        return $"Select the outcome for {command.Heading.ToLower()}";
                    }
            }
        }
    }
}
