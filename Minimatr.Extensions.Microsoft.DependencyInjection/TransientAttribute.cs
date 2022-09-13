using Microsoft.Extensions.DependencyInjection;

namespace Minimatr.Extensions.Microsoft.DependencyInjection;

/// <summary>
/// Mark class as transient service and will automatically be added at application startup.
/// </summary>
public sealed class TransientAttribute : ServiceAttribute {
    public TransientAttribute(Type? service = null) : base(ServiceLifetime.Transient, service) { }
}
