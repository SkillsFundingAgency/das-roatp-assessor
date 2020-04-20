namespace SFA.DAS.RoatpAssessor.Web.Settings
{
    public interface IWebConfiguration
    {
        string SessionRedisConnectionString { get; set; }

        AuthSettings StaffAuthentication { get; set; }

        ClientApiAuthentication RoatpApplicationApiAuthentication { get; set; }
    }
}
