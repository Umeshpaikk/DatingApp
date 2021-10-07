using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extentions
{
    public static class ApplicationServiceExtentions
    {
        public static IServiceCollection AddApplicationServiceExtentions(this IServiceCollection services, IConfiguration Config)
        {
            services.AddSingleton<PresenceTracker>();            
            services.Configure<CloudinarySettings>(Config.GetSection("CloudinarySettings"));
            services.AddScoped<ITokenService, TokenService>();
            /*
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ILikesRepository, LikesRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            */
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<LogUserActivity>();
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            services.AddDbContext<DatatContext>( options =>
            {
                options.UseSqlite(Config.GetConnectionString("DefaultConnection"));
            });

            return services;
        }
    }
}