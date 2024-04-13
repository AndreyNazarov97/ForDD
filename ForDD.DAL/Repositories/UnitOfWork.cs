﻿using ForDD.Domain.Entity;
using ForDD.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace ForDD.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public IBaseRepository<User> Users { get ; set; }
        public IBaseRepository<Role> Roles { get ; set; }
        public IBaseRepository<UserRole> UserRoles { get ; set; }

        public UnitOfWork(ApplicationDbContext context, 
            IBaseRepository<User> users, 
            IBaseRepository<Role> roles, 
            IBaseRepository<UserRole> userRoles)
        {
            _context = context;
            Users = users;
            Roles = roles;
            UserRoles = userRoles;
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    

        public void Dispose()
        {
            
            GC.SuppressFinalize(this);
        }
    }

}
