using SFA.DAS.RoatpAssessor.Web.Infrastructure;

namespace SFA.DAS.RoatpAssessor.Web.Settings;

public class WebConfiguration : IWebConfiguration
{
    public string SessionRedisConnectionString { get; set; }

    public string SessionCachingDatabase { get; set; }

    public string DataProtectionKeysDatabase { get; set; }

    public ManagedIdentityApiAuthentication RoatpApplicationApiAuthentication { get; set; }

    public string EsfaAdminServicesBaseUrl { get; set; }

    public bool UseDfeSignIn { get; set; }

    public string DfESignInServiceHelpUrl { get; set; }
}
