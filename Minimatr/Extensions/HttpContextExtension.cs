namespace Minimatr.Extensions;

public static class HttpContextExtension {
    /// <summary>
    /// Get request parameters from context
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static object? GetRequestBody(this HttpContext context) {
        return context.Items["Request"];
    }

    /// <summary>
    /// Get request parameters from context.
    /// </summary>
    /// <param name="context"></param>
    /// <typeparam name="TRequest"></typeparam>
    /// <returns></returns>
    public static TRequest? GetRequestBody<TRequest>(this HttpContext context) where TRequest : class {
        return context.Items["Request"] as TRequest;
    }
}
