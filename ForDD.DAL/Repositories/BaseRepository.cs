using ForDD.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForDD.DAL.Repositories
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
    {
        private readonly ApplicationDbContext _context;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<TEntity> CreateAsync(TEntity entity)
        {
            if(entity == null)
            {
                throw new ArgumentNullException("Entity is null");
            }

            _context.Add(entity);
            _context.SaveChangesAsync(); ;

            return Task.FromResult(entity);
        }

        public Task<TEntity> DeleteAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Entity is null");
            }

            _context.Remove(entity);
            _context.SaveChangesAsync();

            return Task.FromResult(entity);
        }

        public IQueryable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("Entity is null");
            }

            _context.Update(entity);
            _context.SaveChangesAsync();

            return Task.FromResult(entity);
        }
    }
}
