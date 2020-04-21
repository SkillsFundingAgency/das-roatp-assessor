using Newtonsoft.Json;

namespace SFA.DAS.RoatpAssessor.Web.Settings
{
    public class AuthorizationSettings
    {
        [JsonRequired] public string WtRealm { get; set; }

        [JsonRequired] public string MetadataAddress { get; set; }

        [JsonRequired] public string Role { get; set; }
    }
}
