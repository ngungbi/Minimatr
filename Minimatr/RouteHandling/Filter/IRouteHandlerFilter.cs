namespace Minimatr.RouteHandling.Filter; 

public interface IRouteHandlerFilter {
    ValueTask<object?> InvokeAsync(RouteHandlerInvocationContext context, RouteHandlerFilterDelegate next);
}
