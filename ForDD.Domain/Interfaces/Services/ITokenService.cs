using ForDD.Domain.Dto;
using ForDD.Domain.Result;
using System.Security.Claims;

namespace ForDD.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        string GenerateAccesToken(IEnumerable<Claim> claims);

        string GenerateRefreshToken();


        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);

        Task<BaseResult<TokenDto>> RefreshToken(TokenDto dto);

    }
}
