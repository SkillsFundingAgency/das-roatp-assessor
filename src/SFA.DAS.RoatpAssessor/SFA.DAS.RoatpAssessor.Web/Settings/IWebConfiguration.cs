using SFA.DAS.RoatpAssessor.Web.Infrastructure;

namespace SFA.DAS.RoatpAssessor.Web.Settings;

public interface IWebConfiguration
{
    string SessionRedisConnectionString { get; set; }

    string SessionCachingDatabase { get; set; }

    string DataProtectionKeysDatabase { get; set; }

    ManagedIdentityApiAuthentication RoatpApplicationApiAuthentication { get; set; }

    string EsfaAdminServicesBaseUrl { get; set; }

    string DfESignInServiceHelpUrl { get; set; }
}
