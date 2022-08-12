using MediatR;
using Minimatr.ModelBinding;
using Minimatr.RouteHandling.Filter;

namespace Minimatr.RouteHandling;

internal class RouteExecutor {
    private readonly Type _type;
    private readonly EndpointRouteHandler _handler;


    internal RouteExecutor(Type type, IEnumerable<RouteHandlerFilterAttribute> filters) {
        _type = type;
        var action = new EndpointRouteHandler(EndpointHandler);
        foreach (var item in filters) {
            action = new FilterEndpointRouteHandler(item.InvokeAsync, action.Invoke);
        }

        _handler = action;
    }

    internal async Task Handle(HttpContext context) {
        var routeContext = new DefaultRouteHandlerInvocationContext(context, Array.Empty<object>());
        var request = await context.BindToAsync(_type);
        context.Items.TryAdd("Request", request);
        var result = await _handler.Invoke(routeContext);
        if (result is IResult finalResult) {
            await finalResult.ExecuteAsync(context);
        } else if (result is null) {
            await Results.NoContent().ExecuteAsync(context);
        } else {
            await Results.Ok(result).ExecuteAsync(context);
        }
    }

    private async ValueTask<object?> EndpointHandler(RouteHandlerInvocationContext context) {
        var httpContext = context.HttpContext;
        var sender = httpContext.RequestServices.GetRequiredService<ISender>();
        if (!httpContext.Items.TryGetValue("Request", out var request) || request is null) {
            request = await httpContext.BindToAsync(_type);
        }

        // var request = context.HttpContext.Items[_type]!; // await httpContext.BindTo(_type);
        return await sender.Send(request, httpContext.RequestAborted);
    }
}
