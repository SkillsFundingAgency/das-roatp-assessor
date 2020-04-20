using Newtonsoft.Json;

namespace SFA.DAS.RoatpAssessor.Web.Settings
{
    public class AuthSettings : IAuthSettings
    {
        [JsonRequired] public string WtRealm { get; set; }

        [JsonRequired] public string MetadataAddress { get; set; }
    }
}
