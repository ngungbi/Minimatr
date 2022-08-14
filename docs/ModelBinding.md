# Model Binding

Each property in request can be added with `BindFromAttribute` such as `FromQuery`, `FromForm`, `FromBody`, `FromRoute`, and `FromHeader`.
Each attribute has `Name` property which can be set using constructor to override actual variable name from request.

When no attribute defined, the application will try to guess by checking route pattern.
If the parameter was found at the route pattern, then it will be assumed the parameter is from route, or else
the parameter is coming from query string.
It is recommended to add bind attribute to avoid confusion and unexpected behavior.

> **Important tip:** do not use `Microsoft.AspNetCore.Mvc` namespace.

**Example :**

```csharp
[MapGet("/oauth/{tenantId}/authorize")]
public class AuthorizeRequest : IEndpointRequest {
    [FromRoute]
    public Guid TenantId { get; set; }
    
    [FromQuery("client_id")]
    public Guid ClientId { get; set; }
    
    [FromQuery("redirect_uri")]
    public string? RedirectUri { get; set; }
    
    [FromQuery]
    public string? State { get; set; }
}

public class AuthorizeHandler : IEndpointHandler {
    public async Task<IResult> Handle(AuthorizeRequest request, CancellationToken cancellationToken) {
        // some logics here
        
        return Results.Ok();
    }
}
```

## HttpContext

HttpContext can be added by declaring property with type HttpContext. HttpContext, HttpRequest, and HttpResponse
will bind automatically.

```csharp
[MapPost("/path")]
public class SampleRequest : IEndpointRequest {
    public HttpContext Context { get; set; }
    public HttpRequest Request { get; set; }
    public HttpResnponse Response { get; set; }
} 
```

## FormFile

When property type is `IFormFile` with `FromBody` attribute, it will bind automatically with first `File` from `HttpRequest`

```csharp
[MapPost("/upload")]
public class UploadFileRequest : IEndpointRequest {
    [FromBody]
    public IFormFile UploadedFile { get; set; }
}
```
Or `IFormFIleCollection` for multiple files.
```csharp
[MapPost("/batchupload")]
public class UploadFileRequest : IEndpointRequest {
    [FromBody]
    public IFormFileCollection UploadedFiles { get; set; }
}
```
