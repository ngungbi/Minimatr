using Microsoft.OpenApi.Models;

namespace Minimatr.OpenApi;

public sealed class SchemaGeneratorOptions {
    public SchemaGeneratorOptions(IServiceProvider serviceProvider) {
        ServiceProvider = serviceProvider;
        SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>();
        SecurityRequirements = new List<OpenApiSecurityRequirement>();
    }

    public IServiceProvider ServiceProvider { get; }
    
    /// <summary>
    /// Get or set value for Open API information
    /// </summary>
    public OpenApiInfo Info { get; set; } = new(){ Title = "API Documentation", Version = "v1"};
    public IDictionary<string, OpenApiSecurityScheme> SecuritySchemes { get; set; }
    public IList<OpenApiSecurityRequirement> SecurityRequirements { get; set; }

    /// <summary>
    /// Set Open API info
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public SchemaGeneratorOptions SetInfo(OpenApiInfo info) {
        Info = info;
        return this;
    }

    /// <summary>
    /// Configure Open API information
    /// </summary>
    /// <param name="configure">Configure action</param>
    /// <returns></returns>
    public SchemaGeneratorOptions ConfigureInfo(Action<OpenApiInfo> configure) {
        configure(Info);
        return this;
    }
    
    /// <summary>
    /// Configure Open API information with value from service collection
    /// </summary>
    /// <param name="configure">Configure action</param>
    /// <returns></returns>
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

    /// <summary>
    /// Add bearer authorization schema
    /// </summary>
    /// <param name="configure">Configure action</param>
    /// <returns></returns>
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

    /// <summary>
    /// Add bearer schema
    /// </summary>
    /// <param name="configure">Configure action</param>
    /// <returns></returns>
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

    public Func<Type, SchemaGeneratorOptions, string>? GroupNameConvention { get; set; }

    /// <summary>
    /// Configure custom endpoint grouping
    /// </summary>
    /// <param name="configure">Configure action</param>
    /// <returns></returns>
    public SchemaGeneratorOptions ConfigureGroupName(Func<Type, SchemaGeneratorOptions, string> configure) {
        GroupNameConvention = configure;
        return this;
    }
}
