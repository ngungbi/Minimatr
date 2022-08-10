using Minimatr.RouteHandling.Filter;

namespace Minimatr.RouteHandling;

internal delegate ValueTask<object?> RouteHandlerFilterInvokerDelegate(RouteHandlerInvocationContext context, RouteHandlerFilterDelegate next);
