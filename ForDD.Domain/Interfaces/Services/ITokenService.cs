using System.Security.Claims;

namespace ForDD.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccesToken(IEnumerable<Claim> claims);

        string GenerateRefreshToken();


    }
}
