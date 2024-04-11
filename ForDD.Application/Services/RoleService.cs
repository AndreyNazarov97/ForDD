using AutoMapper;
using ForDD.Application.Mapping;
using ForDD.Application.Resources;
using ForDD.Domain.Dto.Role;
using ForDD.Domain.Entity;
using ForDD.Domain.Enum;
using ForDD.Domain.Interfaces.Repositories;
using ForDD.Domain.Interfaces.Services;
using ForDD.Domain.Result;
using Microsoft.EntityFrameworkCore;

namespace ForDD.Application.Services
{
    public class RoleService : IRoleService
    {
        private readonly IBaseRepository<Role> _roleRepository;
        private readonly IBaseRepository<User> _userRepository;
        private readonly IBaseRepository<UserRole> _userRoleRepository;
        private readonly IMapper _mapper;

        public RoleService(IBaseRepository<Role> roleRepository, 
            IBaseRepository<User> userRepository, 
            IMapper mapper, 
            IBaseRepository<UserRole> userRoleRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto)
        {
            var user = await _userRepository.GetAll().FirstOrDefaultAsync(x => x.Login == dto.Login);
            if (user == null)
            {
                return new BaseResult<UserRoleDto>()
                {
                    ErrorMessage = ErrorMessages.UserNotFound,
                    ErrorCode = (int?)ErrorCodes.UserNotFound
                };
            }

            var roles = user.Roles.Select(x => x.Name).ToArray();

            if(!roles.Any(x => x == dto.RoleName))
            {
                var role = _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.RoleName);
                if(role == null)
                {
                    return new BaseResult<UserRoleDto>()
                    {
                        ErrorMessage = ErrorMessages.RoleNotFound,
                        ErrorCode = (int?)ErrorCodes.RoleNotFound
                    };
                }
                UserRole userRole = new UserRole()
                {
                    RoleId = role.Id,
                    UserId = user.Id,
                };
                await _userRoleRepository.CreateAsync(userRole);

                return new BaseResult<UserRoleDto>()
                {
                    Data = new UserRoleDto(dto.Login, dto.RoleName)
                };
            }

            return new BaseResult<UserRoleDto>()
            {
                ErrorMessage = ErrorMessages.RoleAlreadyExist,
                ErrorCode = (int)ErrorCodes.RoleAlreadyExist
            };

        }

        public async Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
            if (role != null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = ErrorMessages.RoleAlreadyExist,
                    ErrorCode = (int?)ErrorCodes.RoleAlreadyExist
                };
            }

            role = new Role()
            {
                Name = dto.Name
            };
            await _roleRepository.CreateAsync(role);

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

        public async Task<BaseResult<RoleDto>> DeleteRoleAsync(long id)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (role == null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = ErrorMessages.RoleNotExist,
                    ErrorCode = (int?)ErrorCodes.RoleNotExist
                };
            }

           
            await _roleRepository.DeleteAsync(role);

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }

        public async Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (role == null)
            {
                return new BaseResult<RoleDto>()
                {
                    ErrorMessage = ErrorMessages.RoleNotExist,
                    ErrorCode = (int?)ErrorCodes.RoleNotExist
                };
            }

            role.Name = dto.Name;

            await _roleRepository.UpdateAsync(role);

            return new BaseResult<RoleDto>()
            {
                Data = _mapper.Map<RoleDto>(role),
            };
        }
    }
}
