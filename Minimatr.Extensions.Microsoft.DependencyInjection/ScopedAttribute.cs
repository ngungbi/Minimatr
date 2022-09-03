using Microsoft.Extensions.DependencyInjection;

namespace Minimatr.Extensions.Microsoft.DependencyInjection;

public class ScopedAttribute : ServiceAttribute {
    public ScopedAttribute(Type? service = null) : base(ServiceLifetime.Scoped, service) { }
}
