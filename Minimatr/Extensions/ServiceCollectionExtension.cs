using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
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
    public static IServiceCollection AddMinimatr(this IServiceCollection services, Action<MinimatrConfiguration> configure, Action<ObjectParserCollection>? configureParsers = null) {
        // if (services.FirstOrDefault(d => d.ServiceType == typeof(MinimatrConfiguration))?.ImplementationInstance is not MinimatrConfiguration config) {
        //     config = new MinimatrConfiguration();
        //     services.AddSingleton(config);
        //     // var options = Options.Create(config);
        //     // services.AddSingleton(options);
        // }

        services.Configure<MinimatrConfiguration>(
            config => {
                configure(config);
                if (config.Assembly is null) {
                    throw new NullReferenceException("Assembly must be set");
                }
            }
        );

        // configure.Invoke(config);

        return AddMinimatrParsers(services, configureParsers);
    }

    public static IServiceCollection AddMinimatr(this IServiceCollection services, Assembly assembly,
        Action<MinimatrConfiguration>? configure = null,
        Action<ObjectParserCollection>? configureParsers = null) {
        // if (services.FirstOrDefault(d => d.ServiceType == typeof(MinimatrConfiguration))?.ImplementationInstance is not MinimatrConfiguration config) {
        //     config = new MinimatrConfiguration();
        //     // services.Configure(configure);
        //     services.AddSingleton(config);
        //     // var options = Options.Create(config);
        //     // services.AddSingleton(options);
        // }

        // config.Assembly ??= assembly;
        services.Configure<MinimatrConfiguration>(
            configuration => {
                configuration.Assembly ??= assembly;
                configure?.Invoke(configuration);
            }
        );

        // configure?.Invoke(config);

        return AddMinimatrParsers(services, configureParsers);
    }

    private static IServiceCollection AddMinimatrParsers(IServiceCollection services, Action<ObjectParserCollection>? configureParsers) {
        if (services.FirstOrDefault(d => d.ServiceType == typeof(ObjectParserCollection))?.ImplementationInstance is not ObjectParserCollection parserCollection) {
            parserCollection = new ObjectParserCollection();
            services.TryAddSingleton(parserCollection);
        }

        configureParsers?.Invoke(parserCollection);
        ModelBinder.AddDefaultParsers(parserCollection);
        return services;
    }
    // public static IServiceCollection AddParser(this IServiceCollection services, Type type, Func<string, T> configure) {
    //     return services;
    // }
}
