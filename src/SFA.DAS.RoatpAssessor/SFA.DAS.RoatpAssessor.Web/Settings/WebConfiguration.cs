using Newtonsoft.Json;

namespace SFA.DAS.RoatpAssessor.Web.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired]
        public string SessionRedisConnectionString { get; set; }

        [JsonRequired]
        public AuthSettings StaffAuthentication { get; set; }

        [JsonRequired]
        public ClientApiAuthentication RoatpApplicationApiAuthentication { get; set; }
        [JsonRequired]
        public string EsfaAdminServicesBaseUrl { get; set; }
    }
}
