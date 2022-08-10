using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Minimatr.OpenApi;

public static class MinimatrExtension {
    public static IServiceCollection AddMinimatr(this IServiceCollection services, Assembly assembly) {
        services.AddMediatR(assembly);
        return services;
    }

    public static WebApplication MapOpenApiSchema(this WebApplication app, string pattern) {
        app.MapGet(pattern, () => "");
        return app;
    }
}
