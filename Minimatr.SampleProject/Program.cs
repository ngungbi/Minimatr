using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.OpenApi.Models;
using Minimatr;
using Minimatr.Extensions;
using Minimatr.OpenApi;
using Minimatr.SampleProject;

// var pattern = RoutePatternFactory.Parse("/test/{id:int}");

var builder = WebApplication.CreateBuilder(args);
var assembly = Assembly.GetExecutingAssembly();
var services = builder.Services;
services.AddMinimatr(assembly);
services.AddMinimatrSchemaGenerator();
services.AddMediatR(assembly);
// services.AddSwaggerGen();

var app = builder.Build();

app.MapAllRequests();
app.MapOpenApiSchema(
    "/openapi/schema", options => options
        .AddDefaultBearerSchema()
        .AddDefaultSecurityRequirement()
        .SetInfo(
            new OpenApiInfo {
                Title = "Sample Project",
                Version = "1.0.0"
            }
        )
);

app.UseSwaggerUiOptions();

app.Run();
