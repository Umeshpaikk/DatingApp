using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace API.Extentions
{
    public static class IdentityServiceExtentions
    {
        public static IServiceCollection AddIdentityServiceExtentions(this IServiceCollection services, IConfiguration config)
        {

            services.AddIdentityCore<AppUser>(opt =>
            {
                opt.Password.RequireNonAlphanumeric = false;
            })
                .AddRoles<AppRole>()
                .AddRoleManager<RoleManager<AppRole>>()
                .AddSignInManager<SignInManager<AppUser>>()
                .AddRoleValidator<RoleValidator<AppRole>>()
                .AddEntityFrameworkStores<DatatContext>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                options => {
                    options.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWToken"])),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context => {

                        var AccessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;

                        if(!string.IsNullOrEmpty(AccessToken) && path.StartsWithSegments("/hubs"))
                        {
                            context.Token = AccessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAuthorization(opt => {
                    opt.AddPolicy("RequireAdminRoles", policy => policy.RequireRole("Admin"));
                    opt.AddPolicy("ModaratePhotoRoles", policy => policy.RequireRole("Admin", "Moderator"));
            });

            return services;
        }
        
    }
}