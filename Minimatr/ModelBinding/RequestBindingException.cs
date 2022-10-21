namespace Minimatr.ModelBinding;

public sealed class RequestBindingException : Exception {
    public override string Message { get; }

    public RequestBindingException(string? message = null) {
        Message = message ?? string.Empty;
    }
}
