using ForDD.Application.Resources;
using ForDD.Domain.Dto;
using ForDD.Domain.Entity;
using ForDD.Domain.Interfaces.Repositories;
using ForDD.Domain.Interfaces.Services;
using ForDD.Domain.Result;
using ForDD.Domain.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ForDD.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _jwtKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly IBaseRepository<User> _userRepository;

        public TokenService(IOptions<JwtSettings> options, IBaseRepository<User> userRepository)
        {
            _jwtKey = options.Value.JwtKey;
            _issuer = options.Value.Issuer;
            _audience = options.Value.Audience;
            _userRepository = userRepository;
        }

        public string GenerateAccesToken(IEnumerable<Claim> claims)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var securityToken = new JwtSecurityToken(_issuer, _audience, claims, null, DateTime.UtcNow.AddMinutes(10), credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return token;
        }

        public string GenerateRefreshToken()
        {
            var randomNumbers = new Byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();

            randomNumberGenerator.GetBytes(randomNumbers);

            return Convert.ToBase64String(randomNumbers);
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtKey)),
                ValidateLifetime = true,
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var claimsPrincipal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out var securityToken);
            if(securityToken is not JwtSecurityToken jwtSecurityToken 
                || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException(ErrorMessages.InvalidToken);
            }
            return claimsPrincipal;
        }

        public async Task<BaseResult<TokenDto>> RefreshToken(TokenDto dto)
        {
            var accesToken = dto.AccesToken;
            var refreshToken = dto.RefreshToken;

            var claimsPrincipal = GetPrincipalFromExpiredToken(accesToken);
            var userName = claimsPrincipal.Identity?.Name;

            var user = await _userRepository.GetAll()
                .Include(x => x.Token)
                .FirstOrDefaultAsync(x => x.Login == userName);

            if(user == null || user.Token.RefreshToken != refreshToken || user.Token.RefreshTokenExpireTime <= DateTime.UtcNow) 
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = ErrorMessages.InvalidClientRequest,
                };
            }

            var newAccessToken = GenerateAccesToken(claimsPrincipal.Claims);
            var newRefreshToken = GenerateRefreshToken();

            user.Token.RefreshToken = newRefreshToken;
            await _userRepository.UpdateAsync(user);

            return new BaseResult<TokenDto>()
            {
                Data = new TokenDto()
                {
                    AccesToken = newAccessToken,
                    RefreshToken = newRefreshToken,
                }
            };
        }
    }
}
