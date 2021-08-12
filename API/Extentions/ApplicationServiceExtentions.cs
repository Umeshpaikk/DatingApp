using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extentions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationServiceExtentions(this IServiceCollection services, IConfiguration Config)
        {
            
            services.AddScoped<ITokenService, TokenService>();
            services.AddDbContext<DatatContext>( options =>
            {
                options.UseSqlite(Config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}