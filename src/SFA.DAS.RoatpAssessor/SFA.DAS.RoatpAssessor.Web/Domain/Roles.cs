using System.Security.Claims;

namespace SFA.DAS.RoatpAssessor.Web.Domain
{
    public static class Roles
    {
        public const string RoleClaimType = "http://service/service";

        public const string RoatpAssessorTeam = "AAC";

        public static bool HasValidRole(this ClaimsPrincipal user)
        {
            return user.IsInRole(RoatpAssessorTeam);
        }
    }
}
