using Microsoft.Extensions.DependencyInjection;

namespace Minimatr.Extensions.Microsoft.DependencyInjection;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class ServiceAttribute : Attribute {
    public Type? Service { get; }
    public ServiceLifetime Lifetime { get; }

    public ServiceAttribute(ServiceLifetime lifetime, Type? service = null) {
        Lifetime = lifetime;
        Service = service;
    }
}
