namespace SFA.DAS.RoatpAssessor.Web.Settings
{
    public interface IManagedIdentityApiAuthentication
    {
        string Identifier { get; set; }
        string ApiBaseAddress { get; set; }
    }
}