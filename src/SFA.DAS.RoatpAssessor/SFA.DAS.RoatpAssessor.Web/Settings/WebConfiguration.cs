namespace SFA.DAS.RoatpAssessor.Web.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        public string SessionRedisConnectionString { get; set; }
        public AuthorizationSettings StaffAuthentication { get; set; }
    }
}
