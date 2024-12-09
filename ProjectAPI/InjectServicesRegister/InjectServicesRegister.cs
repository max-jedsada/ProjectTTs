using Manager.Interfaces;
using Manager.Services;
using Project.Manager.Redis;
using Provider.Interfaces;
using Provider.Services;

namespace Project.ServicesRegister
{
    public static class InjectServicesRegister
    {
        public static IServiceCollection InjectServices(this IServiceCollection services)
        {
            services.AddScoped<IRedisCacheService, RedisCacheService>();

            // add inject Manager
            services.AddScoped<ITestManager, TestManager>();


            // add inject Provider
            services.AddScoped<ITestProvider, TestProvider>();

            return services;

        }
    }
}
