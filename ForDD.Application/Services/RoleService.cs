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

        public RoleService(IBaseRepository<Role> roleRepository, IBaseRepository<User> userRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
        }

        public async Task<BaseResult<Role>> CreateRoleAsync(RoleDto dto)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Name == dto.Name);
            if (role != null)
            {
                return new BaseResult<Role>()
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

            return new BaseResult<Role>()
            {
                Data = role,
            };
        }

        public async Task<BaseResult<Role>> DeleteRoleAsync(long id)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == id);
            if (role != null)
            {
                return new BaseResult<Role>()
                {
                    ErrorMessage = ErrorMessages.RoleNotExist,
                    ErrorCode = (int?)ErrorCodes.RoleNotExist
                };
            }

           
            await _roleRepository.DeleteAsync(role);

            return new BaseResult<Role>()
            {
                Data = role,
            };
        }

        public async Task<BaseResult<Role>> UpdateRoleAsync(RoleDto dto)
        {
            var role = await _roleRepository.GetAll().FirstOrDefaultAsync(x => x.Id == dto.Id);
            if (role != null)
            {
                return new BaseResult<Role>()
                {
                    ErrorMessage = ErrorMessages.RoleNotExist,
                    ErrorCode = (int?)ErrorCodes.RoleNotExist
                };
            }

            role.Name = dto.Name;

            await _roleRepository.UpdateAsync(role);

            return new BaseResult<Role>()
            {
                Data = role,
            };
        }
    }
}
