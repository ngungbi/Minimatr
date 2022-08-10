using System.Reflection;
using Minimatr.Configuration;
using Minimatr.ModelBinding;

namespace Minimatr.Extensions;

public static class ServiceCollectionExtension {
    // public static IServiceCollection AddMinimatr(this IServiceCollection services, Action<MinimatrConfiguration>? configure = null) {
    //     var config = new MinimatrConfiguration();
    //     configure?.Invoke(config);
    //     services.AddSingleton(config);
    //     return services;
    // }

    public static IServiceCollection AddMinimatr(this IServiceCollection services, Assembly assembly, Action<MinimatrConfiguration>? configure = null, Action<ObjectParserCollection>? configureParsers = null) {
        if (services.FirstOrDefault(d => d.ServiceType == typeof(MinimatrConfiguration))?.ImplementationInstance is not MinimatrConfiguration config) {
            config = new MinimatrConfiguration();
        }

        config.Assembly = assembly;

        configure?.Invoke(config);

        if (services.FirstOrDefault(d => d.ServiceType == typeof(ObjectParserCollection))?.ImplementationInstance is not ObjectParserCollection parserCollection) {
            parserCollection = new ObjectParserCollection();
            services.AddSingleton(parserCollection);
        }

        configureParsers?.Invoke(parserCollection);
        ModelBinder.AddDefaultParsers(parserCollection);
        return services;
    }
    // public static IServiceCollection AddParser(this IServiceCollection services, Type type, Func<string, T> configure) {
    //     return services;
    // }
}
