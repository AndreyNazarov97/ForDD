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
        Task<BaseResult<RoleDto>> CreateRoleAsync(CreateRoleDto dto);

        /// <summary>
        /// Удаление роли
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<BaseResult<RoleDto>> DeleteRoleAsync(long id);

        /// <summary>
        /// Обновление роли
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<RoleDto>> UpdateRoleAsync(RoleDto dto);

        /// <summary>
        /// Добавление роли для пользователя
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<BaseResult<UserRoleDto>> AddRoleForUserAsync(UserRoleDto dto);
    }
}
