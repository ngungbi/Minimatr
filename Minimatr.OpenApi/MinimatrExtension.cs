using System.Reflection;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minimatr.Configuration;

namespace Minimatr.OpenApi;

public static class MinimatrExtension {
    public static IServiceCollection AddMinimatrSchemaGenerator(this IServiceCollection services, Assembly? assembly = null) {
        if (assembly is null) {
            var config = services.First(x => x.ServiceType == typeof(MinimatrConfiguration)).ImplementationInstance as MinimatrConfiguration;
            assembly = config!.Assembly;
        }

        assembly ??= Assembly.GetExecutingAssembly();
        services.AddSingleton(new OpenApiGenerator(assembly));
        return services;
    }

    public static WebApplication MapOpenApiSchema(this WebApplication app, string pattern, Action<SchemaGeneratorOptions>? configure = null) {
        var options = app.Services.GetService<SchemaGeneratorOptions>() ?? new SchemaGeneratorOptions(app.Services);
        configure?.Invoke(options);
        app.MapGet(
            pattern, async ([FromServices] OpenApiGenerator generator) => {
                var document = await generator.GenerateDocument(options);
                return Results.Content(document, "application/json", Encoding.UTF8);
            }
        );
        return app;
    }
    
   
}
