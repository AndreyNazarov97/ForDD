using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForDD.Domain.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity>
    {
        IQueryable<TEntity> GetAll();

        Task<TEntity> CreateAsync(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        Task<TEntity> DeleteAsync(TEntity entity);

    }
}
