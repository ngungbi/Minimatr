using Microsoft.Extensions.DependencyInjection;

namespace Minimatr.Extensions.Microsoft.DependencyInjection;

public sealed class TransientAttribute : ServiceAttribute {
    public TransientAttribute(Type? service = null) : base(ServiceLifetime.Transient, service) { }
}
