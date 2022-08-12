# Setup

## Install dependencies
Install package
```
dotnet add package Minimatr
```
Install MediatR
```
dotnet add package MediatR
```

## Register Minimatr required services;
```csharp
var assembly = Assembly.GetExecutingAssembly();

var builder = WebApplication.CreateBuilder();
var services = builder.Services;
services.AddMinimatr(assembly);
services.AddMediatR(assembly);
```
## Map Endpoints
```csharp
var app = builder.Build();

// this will scan entire assembly for IEndpointRequest with MapMethodAttribute
app.MapAllRequests();

// this will map specific IEndpointRequest to route
app.MapGet<SomeRequest>("/path");
```

## AdvanceSetup
`AddMinimatr` method has overloads to configure settings.
```csharp
services.AddMinimatr(
```
