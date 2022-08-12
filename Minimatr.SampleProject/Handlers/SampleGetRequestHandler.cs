using Minimatr.ModelBinding;

// using Microsoft.AspNetCore.Mvc;

namespace Minimatr.SampleProject.Handlers;

[MapGet("/test/{SampleString}/{RouteInt:int}")]
public class SampleGetRequest : IEndpointRequest {
    // [FromRoute(Name = "RouteString")]
    public string SampleString { get; set; } = string.Empty;

    // [FromQuery]
    public int IntegerValue { get; set; }
    
    public int RouteInt { get; set; }

    [DoNotBind]
    public int Unbinded { get; set; }

    public HttpContext HttpContext { get; set; }
}

public class SampleGetRequestHandler : IEndpointHandler<SampleGetRequest> {
    public async Task<IResult> Handle(SampleGetRequest getRequest, CancellationToken cancellationToken) {
        await Task.Delay(100, cancellationToken);
        return Results.Ok(getRequest);
    }
}
