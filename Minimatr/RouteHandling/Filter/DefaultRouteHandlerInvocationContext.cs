namespace Minimatr.RouteHandling.Filter;

internal class DefaultRouteHandlerInvocationContext : RouteHandlerInvocationContext {
    public override IList<object?> Arguments { get; }
    public override HttpContext HttpContext { get; }

    internal DefaultRouteHandlerInvocationContext(HttpContext httpContext, IEnumerable<object> objects) {
        HttpContext = httpContext;
        Arguments = new List<object?>(objects);
    }

    internal T GetArgument<T>(int index) {
        return (T) Arguments[index]!;
    }
}
