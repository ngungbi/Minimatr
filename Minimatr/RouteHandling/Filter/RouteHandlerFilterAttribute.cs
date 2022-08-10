namespace Minimatr.RouteHandling.Filter;

public abstract class RouteHandlerFilterAttribute : Attribute, IRouteHandlerFilter {
    public abstract ValueTask<object?> InvokeAsync(RouteHandlerInvocationContext context, RouteHandlerFilterDelegate next);
}
