# Routing

## Using `MapMethodAttribute`

`MapGet`, `MapPost`, `MapPut`, and `MapDelete` attributes with its route pattern can be declared on `IEndpointRequest` class.
Each request will be handled by `IEndpointHandler<TRequest>`. 
Using this method, requires `MapAllRequests` to be invoked to `WebApplication` instance.

### Example

Program.cs
```csharp
app = builder.Build();
app.MapAllRequest();
```
Somewhere in project
```csharp
[MapGet("users/{userId:guid}")]
public class ViewUserRequest : IEndpointRequest {
    [FromRoute]
    public Guid UserId { get; set; }
}

public class ViewUserHandler : IEndpointHandler<ViewUserRequest> {
    public async Task<IResult> Handle(ViewUserRequest request, CancellationToken cancellationToken) {
        var userId = request.UserId;
        
        // some logic here
    
        return Results.Ok();
    }
}
```

## Using `Map<TRequest>()`
Mapping can also be done using `Map<TRequest>()` directly to `WebApplication` instance.

Program.cs

```csharp
app = builder.Build();
app.MapPost<CreateUserRequest>("/user");
```

Somewhere in project

```csharp
public class CreateUserRequest : IEndpointRequest {
    [FromBody]
    public User? User { get; set; }
}

public class CreateUserHandler : IEndpointHandler<CreateUserRequest> {
    public async Task<IResult> Handle(CreateUserRequest request, CancellationToken cancellationToken) {
        await Task.Delay(10, cancellationToken);
        var user = request.User;
        if (user is null) return Results.BadRequest();
        user.UserId = Guid.NewGuid();
        return Results.Ok(user);
    }
}
```
