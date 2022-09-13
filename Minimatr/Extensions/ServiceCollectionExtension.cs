using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Minimatr.Configuration;
using Minimatr.ModelBinding;

namespace Minimatr.Extensions;

public static class ServiceCollectionExtension {
    /// <summary>
    /// Add MinimatR to a service collection.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configure">Configure MinimatR options</param>
    /// <param name="configureParsers">Register additional parser for custom type</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static IServiceCollection AddMinimatr(this IServiceCollection services, Action<MinimatrConfiguration> configure, Action<ObjectParserCollection>? configureParsers = null) {
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

    /// <summary>
    /// Add MinimatR to a service collection.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="assembly">Assembly to scan</param>
    /// <param name="configure">Configure MinimatR options</param>
    /// <param name="configureParsers">Register additional parser for custom type</param>
    /// <returns></returns>
    public static IServiceCollection AddMinimatr(this IServiceCollection services, Assembly assembly,
        Action<MinimatrConfiguration>? configure = null,
        Action<ObjectParserCollection>? configureParsers = null) {
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
}
