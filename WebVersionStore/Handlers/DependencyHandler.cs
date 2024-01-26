using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebVersionStore.Models;
using WebVersionStore.Models.Local;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebVersionStore.Handlers
{
    public static class DependencyHandler
    {
        public static IServiceCollection AddDefaultDependencies(this IServiceCollection services, ConfigurationManager config)
        {
            services.AddDbContext<WebVersionControlContext>(options =>
                options.UseSqlServer(config.GetConnectionString("WebVersionStoreDatabase")));


            return services;
        }
        public static AuthenticationBuilder SetupJwtBearer(this AuthenticationBuilder builder, IConfigurationSection jwtConfiguration)
        {
            return builder.AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = context =>
                    {
                        context.ValidateJwtToken();
                        return Task.CompletedTask;
                    }
                };
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,                                   
                    IssuerSigningKey = new SymmetricSecurityKey(
                        JwtSettings.AsBytes(jwtConfiguration.GetValue<string>("Secret")!)
                        ),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }
    }
}
