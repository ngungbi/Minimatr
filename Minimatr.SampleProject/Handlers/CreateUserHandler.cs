using Minimatr.ModelBinding;
using Minimatr.SampleProject.Models;

namespace Minimatr.SampleProject.Handlers;

[MapPost("/users")]
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
