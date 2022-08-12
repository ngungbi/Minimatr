using Minimatr.ModelBinding;

namespace Minimatr.SampleProject.Handlers;

[MapPost("upload")]
public class UploadFileRequest : IEndpointRequest {
    [FromForm]
    public string? FileName { get; set; }

    [FromBody]
    public IFormFile? File { get; set; }
}

public class UploadFileHandler : IEndpointHandler<UploadFileRequest> {
    public Task<IResult> Handle(UploadFileRequest request, CancellationToken cancellationToken) {
        var result = new {
            fileName = request.FileName,
            length = request.File?.Length
        };
        return Task.FromResult(Results.Ok(result));
    }
}
