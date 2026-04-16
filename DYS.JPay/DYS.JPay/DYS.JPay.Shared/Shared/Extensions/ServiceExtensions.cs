using DYS.JPay.Shared.Shared.Services;
using DYS.JPay.Shared.Shared.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Reflection;
using System.Security.AccessControl;

namespace DYS.JPay.Shared.Shared.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddScopedForBaseClasses(this IServiceCollection services, Assembly assembly)
        {
            // Find all concrete classes that implement IBaseService
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IBaseService).IsAssignableFrom(t));

            foreach (var implType in types)
            {
                // Find the first interface that matches naming convention (e.g. ISaleService for SaleService)
                var serviceInterface = implType.GetInterfaces()
                    .FirstOrDefault(i => i.Name == $"I{implType.Name}");

                if (serviceInterface != null)
                {
                    services.AddScoped(serviceInterface, implType);
                }
                else
                {
                    // fallback: register the class itself
                    services.AddScoped(implType);
                }
            }

            return services;
        }


        public static IServiceCollection AddTransientForViewModels(this IServiceCollection services, Assembly assembly)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseViewModel)));

            services.AddTransient<BaseViewModel>();
            foreach (var type in types) {
                services.AddTransient(type);
            }
            return services;
        }

        public static IServiceCollection AddTransientForServices<TBase, TInterface>(this IServiceCollection services, Assembly assembly)
        {
            var baseType = typeof(TBase);
            var baseInterfaceType = typeof(TInterface);
            // Find all classes that inherit from the base class
            var types = assembly.GetTypes()
               .Where(type => type.IsClass
                              && !type.IsAbstract
                              && baseType.IsAssignableFrom(type)
                              && type.GetInterfaces().Any(i => i != baseInterfaceType && baseInterfaceType.IsAssignableFrom(i)));


            // Register each derived type with scoped lifetime
            foreach (var type in types)
            {
                var implementedInterface = type.GetInterfaces()
                   .FirstOrDefault(i => i != baseInterfaceType && baseInterfaceType.IsAssignableFrom(i));

                if (implementedInterface != null)
                {
                    services.AddTransient(implementedInterface, type);
                }
            }
            return services;
        }


        public static IServiceCollection AddOtherServices(this IServiceCollection serviceCollection)
        {
            //erviceCollection.AddSingleton<IMapper, Mapper>();
            //serviceCollection.AddScoped<IRequestProvider, RequestProvider>();
            return serviceCollection;
        }
    }
}
