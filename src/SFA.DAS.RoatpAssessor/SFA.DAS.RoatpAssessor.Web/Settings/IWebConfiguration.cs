using SFA.DAS.AdminService.Common.Settings;

namespace SFA.DAS.RoatpAssessor.Web.Settings
{
    public interface IWebConfiguration
    {
        string SessionRedisConnectionString { get; set; }

        AuthSettings StaffAuthentication { get; set; }

        ManagedIdentityApiAuthentication RoatpApplicationApiAuthentication { get; set; }

        string EsfaAdminServicesBaseUrl { get; set; }
    }
}
