# Minimatr - Minimal API with MediatR

Create a clean architecture in dotnet minimal API using MediatR.

## Features

- Simple implementation
- OpenAPI documentation support

## Quick Start

Program.cs

```csharp
var builder = WebApplication.CreateBuilder(args);
var assembly = Assembly.GetExecutingAssembly();
var services = builder.Services;

services.AddMinimatr(assembly);
services.AddMediatR(assembly);

var app = builder.Build();
app.MapAllRequests();

app.Run();
```

Somewhere in project

```csharp
[MapGet("/hello/{id}")]
public class SampleGetRequest : IEndpointRequest {
    [FromRoute]
    public Guid Id { get; set; }
    
    [FromQuery]
    public string? Value { get; set; }
    
}

public class SampleGetRequestHandler : IEndpointHandler<SampleGetRequest> {
    public async Task<IResult> Handle(SampleGetRequest request, CancellationToken cancellationToken) {
        return Results.Ok(request);
    }
}
```
