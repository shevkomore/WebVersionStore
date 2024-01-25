using WebVersionStore.Models;

namespace WebVersionStore.Handlers
{
    public static class DependencyHandler
    {
        public static IServiceCollection AddDefaultDependencies(this IServiceCollection services)
        {
            services.AddScoped<WebVersionControlContext>();
            //services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

            return services;
        }
    }
}
