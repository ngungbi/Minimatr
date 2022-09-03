using System.Reflection;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Minimatr.Configuration;

namespace Minimatr.OpenApi;

public static class MinimatrExtension {
    public static IServiceCollection AddMinimatrSchemaGenerator(this IServiceCollection services, IConfiguration configuration) {
        services.TryAddSingleton<IOptions<SchemaGeneratorOptions>>(serviceProvider => Options.Create(new SchemaGeneratorOptions(serviceProvider)));
        services.Configure<SchemaGeneratorOptions>(configuration.Bind);
        // services.AddSingleton(new OpenApiGenerator(assembly));
        services.TryAddSingleton<OpenApiGenerator>();
        return services;
    }

    public static IServiceCollection AddMinimatrSchemaGenerator(this IServiceCollection services, Action<SchemaGeneratorOptions>? configure = null) {
        services.TryAddSingleton<IOptions<SchemaGeneratorOptions>>(serviceProvider => Options.Create(new SchemaGeneratorOptions(serviceProvider)));
        services.Configure<SchemaGeneratorOptions>(options => configure?.Invoke(options));
        // services.AddSingleton(new OpenApiGenerator(assembly));
        services.TryAddSingleton<OpenApiGenerator>();
        return services;
    }

    public static IServiceCollection AddMinimatrSchemaGenerator(this IServiceCollection services, Assembly? assembly, Action<SchemaGeneratorOptions>? configure = null)
        => AddMinimatrSchemaGenerator(services, configure);
    // {
    //     // if (assembly is null) {
    //     //     // var config = services.First(x => x.ServiceType == typeof(MinimatrConfiguration)).ImplementationInstance as MinimatrConfiguration;
    //     //     // assembly = config!.Assembly;
    //     //     services.Configure<MinimatrConfiguration>(config => assembly = config.Assembly);
    //     // }
    //
    //     // assembly ??= Assembly.GetExecutingAssembly();
    //     services.AddSingleton<IOptions<SchemaGeneratorOptions>>(serviceProvider => Options.Create(new SchemaGeneratorOptions(serviceProvider)));
    //     services.Configure<SchemaGeneratorOptions>(options => configure?.Invoke(options));
    //     // services.AddSingleton(new OpenApiGenerator(assembly));
    //     services.TryAddSingleton<OpenApiGenerator>();
    //     return services;
    // }

    public static WebApplication MapOpenApiSchema(this WebApplication app, string pattern, Action<SchemaGeneratorOptions>? configure = null) {
        var options = app.Services.GetRequiredService<IOptions<SchemaGeneratorOptions>>().Value;

        //?.Value ?? new SchemaGeneratorOptions {ServiceProvider = app.Services};
        configure?.Invoke(options);
        app.MapGet(
            pattern, async ([FromServices] OpenApiGenerator generator) => {
                var document = await generator.GenerateDocument();
                return Results.Content(document, "application/json", Encoding.UTF8);
            }
        );
        return app;
    }
}
