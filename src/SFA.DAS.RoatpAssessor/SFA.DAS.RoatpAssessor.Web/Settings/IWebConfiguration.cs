namespace SFA.DAS.RoatpAssessor.Web.Settings
{
    public interface IWebConfiguration
    {
        string SessionRedisConnectionString { get; set; }

        ClientApiAuthentication RoatpApplicationApiAuthentication { get; set; }
    }
}
