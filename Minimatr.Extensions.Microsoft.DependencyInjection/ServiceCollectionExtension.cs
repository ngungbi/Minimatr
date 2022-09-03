using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Minimatr.Extensions.Microsoft.DependencyInjection;

public static class ServiceCollectionExtension {
    public static IServiceCollection AddRequiredServices(this IServiceCollection services, Assembly assembly) {
        var types = assembly.GetTypes();
        foreach (var item in types) {
            var attr = item.GetCustomAttribute<ServiceAttribute>();
            if (attr is null) continue;

            if (attr.Service is null) {
                services.Add(new ServiceDescriptor(item, item, attr.Lifetime));
            } else {
                var descriptor = item.IsInterface
                    ? new ServiceDescriptor(item, attr.Service, attr.Lifetime)
                    : new ServiceDescriptor(attr.Service, item, attr.Lifetime);

                services.Add(descriptor);
            }
        }

        return services;
    }
}
