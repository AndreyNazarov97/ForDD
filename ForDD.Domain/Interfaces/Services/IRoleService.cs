using ForDD.Domain.Dto.Role;
using ForDD.Domain.Entity;
using ForDD.Domain.Result;

namespace ForDD.Domain.Interfaces.Services
{
    /// <summary>
    /// Сервис для управления ролями
    /// </summary>
    public interface IRoleService
    {
        /// <summary>
        /// Создание роли
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<Role>> CreateRoleAsync(RoleDto dto);

        /// <summary>
        /// Удаление роли
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResult<Role>> DeleteRoleAsync(long id);
        
        /// <summary>
        /// Обновление роли
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<Role>> UpdateRoleAsync(RoleDto dto);
    }
}
