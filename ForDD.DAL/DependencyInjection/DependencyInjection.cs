using ForDD.DAL.Interceptors;
using ForDD.DAL.Repositories;
using ForDD.Domain.Entity;
using ForDD.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ForDD.DAL.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddDataAccesLayer(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("PostgreSQL");

            services.AddSingleton<DateInterceptor>();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            }); 

            InitRepositories(services);
        }

        private static void InitRepositories(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
            services.AddScoped<IBaseRepository<Role>, BaseRepository<Role>>();
            services.AddScoped<IBaseRepository<UserRole>, BaseRepository<UserRole>>();
            services.AddScoped<IBaseRepository<Report>, BaseRepository<Report>>();
            services.AddScoped<IBaseRepository<UserToken>, BaseRepository<UserToken>>();

            
        }

    }
}
