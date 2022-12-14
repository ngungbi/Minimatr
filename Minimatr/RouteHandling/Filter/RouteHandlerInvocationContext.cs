namespace Minimatr.RouteHandling.Filter;

public abstract class RouteHandlerInvocationContext {
    public abstract IList<object?> Arguments { get; }
    public abstract HttpContext HttpContext { get; }
}
