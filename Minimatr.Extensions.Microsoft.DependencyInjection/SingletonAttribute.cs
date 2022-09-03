using Microsoft.Extensions.DependencyInjection;

namespace Minimatr.Extensions.Microsoft.DependencyInjection;

public class SingletonAttribute : ServiceAttribute {
    public SingletonAttribute(Type? service = null) : base(ServiceLifetime.Singleton, service) { }
}
