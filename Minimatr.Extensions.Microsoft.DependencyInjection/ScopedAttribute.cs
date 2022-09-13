using Microsoft.Extensions.DependencyInjection;

namespace Minimatr.Extensions.Microsoft.DependencyInjection;

public sealed class ScopedAttribute : ServiceAttribute {
    public ScopedAttribute(Type? service = null) : base(ServiceLifetime.Scoped, service) { }
}
