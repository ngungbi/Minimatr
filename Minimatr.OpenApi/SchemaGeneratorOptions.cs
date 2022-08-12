using Microsoft.OpenApi.Models;

namespace Minimatr.OpenApi;

public class SchemaGeneratorOptions {
    public SchemaGeneratorOptions(IServiceProvider serviceProvider) {
        ServiceProvider = serviceProvider;
        SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>();
        SecurityRequirements = new List<OpenApiSecurityRequirement>();
    }

    public IServiceProvider ServiceProvider { get; }
    public OpenApiInfo Info { get; set; } = new();
    public IDictionary<string, OpenApiSecurityScheme> SecuritySchemes { get; set; }
    public IList<OpenApiSecurityRequirement> SecurityRequirements { get; set; }

    public SchemaGeneratorOptions SetInfo(OpenApiInfo info) {
        Info = info;
        return this;
    }

    public SchemaGeneratorOptions ConfigureInfo(Action<OpenApiInfo> configure) {
        configure(Info);
        return this;
    }

    public SchemaGeneratorOptions ConfigureInfo(Action<OpenApiInfo, IServiceProvider> configure) {
        configure(Info, ServiceProvider);
        return this;
    }

    public SchemaGeneratorOptions AddSecurityScheme(string name, OpenApiSecurityScheme scheme) {
        SecuritySchemes.Add(name, scheme);
        return this;
    }

    public SchemaGeneratorOptions AddSecurityScheme(string name, Action<OpenApiSecurityScheme, IServiceProvider> configure) {
        var scheme = new OpenApiSecurityScheme();
        configure(scheme, ServiceProvider);
        SecuritySchemes.Add(name, scheme);
        return this;
    }

    public SchemaGeneratorOptions AddSecurityRequirement(OpenApiSecurityRequirement scheme) {
        SecurityRequirements.Add(scheme);
        return this;
    }

    public SchemaGeneratorOptions AddSecurityRequirement(Action<OpenApiSecurityRequirement, IServiceProvider> configure) {
        var scheme = new OpenApiSecurityRequirement();
        configure(scheme, ServiceProvider);
        SecurityRequirements.Add(scheme);
        return this;
    }

    public SchemaGeneratorOptions AddDefaultBearerSchema(Action<OpenApiSecurityScheme>? configure = null) {
        var scheme = new OpenApiSecurityScheme {
            In = ParameterLocation.Header,
            Name = "Authorization",
            Scheme = "bearer",
            Type = SecuritySchemeType.Http
        };
        configure?.Invoke(scheme);
        AddSecurityScheme("bearer", scheme);
        return this;
    }

    public SchemaGeneratorOptions AddDefaultSecurityRequirement(Action<OpenApiSecurityRequirement>? configure = null) {
        var requirement = new OpenApiSecurityRequirement {
            {
                new OpenApiSecurityScheme {
                    Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "bearer"}
                },
                Array.Empty<string>()
            }
        };
        configure?.Invoke(requirement);
        AddSecurityRequirement(requirement);
        return this;
    }
}
