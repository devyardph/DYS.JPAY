using DYS.JPay.Server.Shared.Shared.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Diagnostics;
using System.Reflection;
using System.Security.AccessControl;

namespace DYS.JPay.Server.Shared.Extensions
{
    public static class ServiceExtensions
    {
        public static void AddScopedForBaseClass<TBase, TInterface>(this IServiceCollection services, Assembly assembly)
        {
            var baseClassType = typeof(TBase);
            var baseInterfaceType = typeof(TInterface);

            // Find all classes that inherit from the base class and implement an interface derived from the base interface
            var types = assembly.GetTypes()
                .Where(type => type.IsClass
                               && !type.IsAbstract
                               && baseClassType.IsAssignableFrom(type)
                               && type.GetInterfaces().Any(i => i != baseInterfaceType && baseInterfaceType.IsAssignableFrom(i)));

            // Register each type with the DI container
            foreach (var type in types)
            {
                var implementedInterface = type.GetInterfaces()
                    .FirstOrDefault(i => i != baseInterfaceType && baseInterfaceType.IsAssignableFrom(i));

                if (implementedInterface != null)
                {
                    services.AddScoped(implementedInterface, type);
                }
            }
        }

        public static void AddScopedForBaseClass<TBase>(this IServiceCollection services, Assembly assembly)
        {
            var baseType = typeof(TBase);
          
            // Find all classes that inherit from the base class
            var types = assembly.GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && baseType.IsAssignableFrom(type));

            // Register each derived type with scoped lifetime
            foreach (var type in types) services.AddScoped(type);
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
