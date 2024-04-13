using ForDD.Domain.Entity;
using Microsoft.EntityFrameworkCore.Storage;

namespace ForDD.Domain.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        Task<IDbContextTransaction> BeginTransactionAsync();

        Task<int> SaveChangesAsync();

        IBaseRepository<User> Users { get; set; }

        IBaseRepository<Role> Roles { get; set; }
        IBaseRepository<UserRole> UserRoles { get; set; }
    }
}
