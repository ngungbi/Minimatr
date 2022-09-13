using Microsoft.Extensions.DependencyInjection;

namespace Minimatr.Extensions.Microsoft.DependencyInjection;

/// <summary>
/// Mark class as scoped service and will automatically be added at application startup.
/// </summary>
public sealed class ScopedAttribute : ServiceAttribute {
    public ScopedAttribute(Type? service = null) : base(ServiceLifetime.Scoped, service) { }
}
