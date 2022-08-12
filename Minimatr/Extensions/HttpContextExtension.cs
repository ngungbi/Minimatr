namespace Minimatr.Extensions;

public static class HttpContextExtension {
    public static object? GetRequestBody(this HttpContext context) {
        return context.Items["Request"];
    }

    public static TRequest? GetRequestBody<TRequest>(this HttpContext context) where TRequest : class {
        return context.Items["Request"] as TRequest;
    }
}
