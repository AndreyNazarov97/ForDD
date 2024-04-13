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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<UserToken> _userTokenRepository;
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AuthService(IBaseRepository<User> userRepository,
            IBaseRepository<UserToken> userTokenRepository,
            ITokenService tokenService,
            ILogger logger,
            IMapper mapper,
            IBaseRepository<Role> roleRepository,
            IBaseRepository<UserRole> userRoleRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _userTokenRepository = userTokenRepository;
            _tokenService = tokenService;
            _mapper = mapper;
            _roleRepository = roleRepository;
            _unitOfWork = unitOfWork;
        }



        /// <inheritdoc/>
        public async Task<BaseResult<TokenDto>> Login(LoginUserDto dto)
        {
            var user = await _userRepository.GetAll()
                    .Include(x => x.Roles)
                    .FirstOrDefaultAsync(x => x.Login == dto.Login);
            if (user == null)
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = ErrorMessages.UserNotFound,
                    ErrorCode = ((int)ErrorCodes.UserNotFound)
                };
            }

            if (!IsVerifyPassword(dto.Password, user.Password))
            {
                return new BaseResult<TokenDto>()
                {
                    ErrorMessage = ErrorMessages.WrongPassword,
                    ErrorCode = ((int)ErrorCodes.WrongPassword)
                };
            }

            var userToken = await _userTokenRepository.GetAll().FirstOrDefaultAsync(x => x.UserId == user.Id);

            var userRoles = user.Roles;
            var claims = userRoles.Select(x => new Claim(ClaimTypes.Role, x.Name)).ToList();
            claims.Add(new Claim(ClaimTypes.Name, user.Login));

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

                var updatedRole = _userTokenRepository.Update(userToken);
                await _userTokenRepository.SaveChangesAsync();

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


            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Login);
            if (user != null)
            {
                return new BaseResult<UserDto>()
                {
                    ErrorMessage = ErrorMessages.UserAlreadyExist,
                    ErrorCode = ((int)ErrorCodes.UserAlreadyExist)
                };
            }
            var hashUserPassword = HashPassword(dto.Password);

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    user = new User()
                    {
                        Login = dto.Login,
                        Password = hashUserPassword,

                    };
                    await _unitOfWork.Users.CreateAsync(user);
                    await _unitOfWork.SaveChangesAsync();

                    var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == nameof(Roles.User));
                    if (role == null)
                    {
                        return new BaseResult<UserDto>()
                        {
                            ErrorMessage = ErrorMessages.RoleNotFound,
                            ErrorCode = ((int)ErrorCodes.RoleNotFound)
                        };
                    }
                    UserRole userRole = new UserRole()
                    {
                        UserId = user.Id,
                        RoleId = role.Id,
                    };
                    await _unitOfWork.UserRoles.CreateAsync(userRole);
                    await _unitOfWork.SaveChangesAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                }
            }

            return new BaseResult<UserDto>()
            {
                Data = _mapper.Map<UserDto>(user)
            };
        }

        private string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool IsVerifyPassword(string password, string hashedPassword)
        {
            var hash = HashPassword(password);
            
            return hash == hashedPassword;
        }
    }
}
