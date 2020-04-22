using System;

namespace SFA.DAS.RoatpAssessor.Web.Infrastructure.ApiClients.TokenService
{
    public interface ITokenService
    {
        string GetToken(Uri baseUri);
    }
}
