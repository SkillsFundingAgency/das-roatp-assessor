using Newtonsoft.Json;
using SFA.DAS.AdminService.Common.Settings;

namespace SFA.DAS.RoatpAssessor.Web.Settings
{
    public class WebConfiguration : IWebConfiguration
    {
        [JsonRequired]
        public string SessionRedisConnectionString { get; set; }

        [JsonRequired]
        public string SessionCachingDatabase { get; set; }

        [JsonRequired]
        public string DataProtectionKeysDatabase { get; set; }

        [JsonRequired]
        public AuthSettings StaffAuthentication { get; set; }

        [JsonRequired]
        public ManagedIdentityApiAuthentication RoatpApplicationApiAuthentication { get; set; }

        [JsonRequired]
        public string EsfaAdminServicesBaseUrl { get; set; }
    }
}
