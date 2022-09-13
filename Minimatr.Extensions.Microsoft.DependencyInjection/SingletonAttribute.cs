using Microsoft.Extensions.DependencyInjection;

namespace Minimatr.Extensions.Microsoft.DependencyInjection;

/// <summary>
/// Mark class as singleton service and will automatically be added at application startup.
/// </summary>
public sealed class SingletonAttribute : ServiceAttribute {
    public SingletonAttribute(Type? service = null) : base(ServiceLifetime.Singleton, service) { }
}
