using FluentValidation;
using ForDD.Application.Mapping;
using ForDD.Application.Services;
using ForDD.Application.Validations;
using ForDD.Application.Validations.FluentValidations.Report;
using ForDD.Domain.Dto.Report;
using ForDD.Domain.Interfaces.Services;
using ForDD.Domain.Interfaces.Validations;
using Microsoft.Extensions.DependencyInjection;

namespace ForDD.Application.DependencyInjection
{
    public static class DependencyInjection
    {
        public static void AddApplication(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ReportMapping));
            services.AddAutoMapper(typeof(UserMapping));

            InitServices(services);
        }


        private static void InitServices(this IServiceCollection services)
        {
            services.AddScoped<IReportValidator, ReportValidator>();
            services.AddScoped<IValidator<CreateReportDto>, CreateReportValidator>();
            services.AddScoped<IValidator<UpdateReportDto>, UpdateReportValidator>();
            
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();


        }
    }
}
