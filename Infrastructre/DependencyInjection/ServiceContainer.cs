using ApplicationLibrary.Contract;
using Domain.Entity.Authentication;
using Infrastructre.Data;
using Infrastructre.Repoes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructre.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(_o_ => _o_.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            services.AddIdentityCore<ApplicationUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddSignInManager();

            services.AddAuthentication(_opt_ =>
            {
                _opt_.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                _opt_.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidIssuer = config["JWT:Issuer"],
                    ValidAudience = config["JWT:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"]!))
                };
            });

            services.AddAuthentication();
            services.AddAuthorization();

            services.AddCors(option =>
            {
                option.AddPolicy("WebUI", builder =>
                builder.WithOrigins("https://localhost:7005")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
                );
            });
            services.AddScoped<IAccount, AccountRepository>();
            return services;
        }
    }
}
