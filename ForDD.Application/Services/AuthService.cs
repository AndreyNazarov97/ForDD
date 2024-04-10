using AutoMapper;
using ForDD.Application.Resources;
using ForDD.Domain.Dto;
using ForDD.Domain.Dto.Report;
using ForDD.Domain.Dto.User;
using ForDD.Domain.Entity;
using ForDD.Domain.Enum;
using ForDD.Domain.Interfaces.Repositories;
using ForDD.Domain.Interfaces.Services;
using ForDD.Domain.Result;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ForDD.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<UserToken> _userTokenRepository;
        private readonly ITokenService _tokenService;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public AuthService(IBaseRepository<User> userRepository, 
            IBaseRepository<UserToken> userTokenRepository, 
            ITokenService tokenService,
            ILogger logger, 
            IMapper mapper)
        {
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _tokenService = tokenService;
            _logger = logger;
            _mapper = mapper;
        }



        /// <inheritdoc/>
        public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
        {
            try
            {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Login);
                if (user == null)
                {
                    return new BaseResult<TokenDto>()
                    {
                        ErrorMessage = ErrorMessages.UserNotFound,
                        ErrorCode = ((int)ErrorCodes.UserNotFound)
                    };
                }

                if (!IsVerifyPassword(user.Password, dto.Password))
                {
                    return new BaseResult<TokenDto>()
                    {
                        ErrorMessage = ErrorMessages.WrongPassword,
                        ErrorCode = ((int)ErrorCodes.WrongPassword)
                    };
                }

                var userToken = await _userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);

                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.Login),
                    new Claim(ClaimTypes.Role, "User"),
                };
                var accesToken = _tokenService.GenerateAccesToken(claims);
                var refreshToken = _tokenService.GenerateRefreshToken();

                if (userToken == null)
                {
                    userToken = new UserToken()
                    {
                        UserId = user.Id,
                        RefreshToken = refreshToken,
                        RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7),

                    };
                    await _userTokenRepository.CreateAsync(userToken);
                }
                else
                {
                    userToken.RefreshToken = refreshToken;
                    userToken.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);

                }

                return new BaseResult<TokenDto>()
                {
                    Data = new TokenDto()
                    {
                        RefreshToken = refreshToken,
                        AccesToken = accesToken,
                    }
                };
            }

            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);

                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = ErrorMessages.InternalServerError,
                    ErrorCode = ((int)ErrorCodes.InternalServerError)
                };
            }

        }

        /// <inheritdoc/>
        public async Task<BaseResult<UserDto>> Register(RegisterUserDto dto)
        {
            if(dto.Password != dto.PasswordConfirm)
            {
                return new BaseResult<UserDto>()
                {
                    ErrorMessage = ErrorMessages.PasswordsNotEqual,
                    ErrorCode = (int?)ErrorCodes.PasswordsNotEqual
                };
            }

            try
            {
                var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Login);
                if(user != null)
                {
                    return new BaseResult<UserDto>()
                    {
                        ErrorMessage = ErrorMessages.UserAlreadyExist,
                        ErrorCode = ((int)ErrorCodes.UserAlreadyExist)
                    };
                }
                var hashUserPassword = HashPassword(dto.Password);
                user = new User()
                {
                    Login = dto.Login,
                    Password = hashUserPassword
                };
                await _userRepository.CreateAsync(user);
                return new BaseResult<UserDto>()
                {
                    Data = _mapper.Map<UserDto>(user)
                };

            }
            catch(Exception ex)
            {
                _logger.Error(ex, ex.Message);

                return new BaseResult<UserDto>()
                {
                    ErrorMessage = ErrorMessages.InternalServerError,
                    ErrorCode = ((int)ErrorCodes.InternalServerError)
                };
            }

            
        }

        private string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).ToLower();
        }

        private bool IsVerifyPassword(string password, string hashedPassword)
        {
            var hash = HashPassword(password);
            
            return hash == hashedPassword;
        }
    }
}
