# Filters

Filter can be added using `AddFilterAttribute(Type filterType)`. Filter must implement `IRouteHandlerFilter`.
Multiple filters can be added to a single request endpoint and will be executed from top to bottom.

```csharp
public class ExampleFilter : IRouteHandlerFilter {
    public async ValueTask<object?> InvokeAsync(RouteHandlerInvocationContext context, RouteHandlerFilterDelegate next) {
        // some logic here
        
        return await next(context);
    }
}
```
```csharp
[AddFilter(typeof(ExampleFilter)]
public class SampleRequest : IEndpointRequest {
    // some parameters   
}

public class SampleRequestHandler : IEndpointRequestHandler {
    public async Task<IResult> Handle(SampleRequest request, CancellationToken cancellationToken) {
        // some logic here
        
        return Results.Ok();
    }
}
```
## Using filter attribute
If a filter type is also an attribute, it can be added directly to request class.
```csharp
public class ExampleFilterAttribute : Attribute, IRouteHandlerFilter {
    public async ValueTask<object?> InvokeAsync(RouteHandlerInvocationContext context, RouteHandlerFilterDelegate next) {
        // some logic here
        
        return await next(context);
    }
}
```
```csharp
[ExampleFilter]
public class SampleRequest : IEndpointRequest {
    // some parameters   
}

public class SampleRequestHandler : IEndpointRequestHandler {
    public async Task<IResult> Handle(SampleRequest request, CancellationToken cancellationToken) {
        // some logic here
        
        return Results.Ok();
    }
}
```
## Retrieve request object
Request parameters was saved to HttpContext.Items and can be retrieved in filter using `GetRequest()`
```csharp
public class ExampleFilterAttribute : Attribute, IRouteHandlerFilter {
    public async ValueTask<object?> InvokeAsync(RouteHandlerInvocationContext context, RouteHandlerFilterDelegate next) {
        var request = context.GetRequest<SampleRequest>();
        if (request is null) return Results.BadRequest();
        
        // some logics here
        
        return await next(context);
    }
}
```
