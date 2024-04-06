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
            var connectionString = configuration.GetConnectionString("MSSQL");

            services.AddSingleton<DateInterceptor>();
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            }); 

            InitRepositories(services);
        }

        private static void InitRepositories(this IServiceCollection services)
        {
            services.AddScoped<IBaseRepository<User>, BaseRepository<User>>();
            services.AddScoped<IBaseRepository<Report>, BaseRepository<Report>>();

            
        }

    }
}
