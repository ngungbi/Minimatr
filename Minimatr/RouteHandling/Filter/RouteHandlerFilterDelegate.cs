namespace Minimatr.RouteHandling.Filter;

public delegate ValueTask<object?> RouteHandlerFilterDelegate(RouteHandlerInvocationContext context);
