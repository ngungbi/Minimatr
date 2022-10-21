using System.Text.Json;
using MediatR;
using Minimatr.ModelBinding;
using Minimatr.RouteHandling.Filter;

namespace Minimatr.RouteHandling;

internal sealed class RouteExecutor {
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

    private static ILogger<RouteExecutor> GetLogger(HttpContext context)
        => context.RequestServices.GetRequiredService<ILogger<RouteExecutor>>();

    internal async Task Handle(HttpContext context) {
        IResult response;
        try {
            var request = await context.BindToAsync(_type); //.ConfigureAwait(false);
            var routeContext = new DefaultRouteHandlerInvocationContext(context, request);
            // routeContext.Request = request;
            // routeContext.Arguments.Add(request);
            // context.Items.TryAdd("Request", request);
            var result = await _handler.Invoke(routeContext);
            response = result switch {
                IResult finalResult => finalResult,
                null => Results.NoContent(),
                _ => Results.Ok(result)
            };
        } catch (JsonException e) {
            response = Results.BadRequest();
            GetLogger(context).LogWarning("Failed to process JSON request for {Type}: {Message}", _type.FullName, e.Message);
        } catch (RequestBindingException e) {
            response = Results.BadRequest();
            GetLogger(context).LogWarning("Failed to process request for {Type}: {Message}", _type.FullName, e.Message);
            // } catch (Exception e) {
            //     response = Results.StatusCode(500);
            //     var logger = GetLogger(context);
            //     logger.LogError("Failed to process request: {Message}", e.Message);
            //     if (logger.IsEnabled(LogLevel.Information)) {
            //         Console.WriteLine(e);
            //     }
        }

        await response.ExecuteAsync(context).ConfigureAwait(false);
    }

    private async ValueTask<object?> EndpointHandler(RouteHandlerInvocationContext context) {
        var httpContext = context.HttpContext;
        var sender = httpContext.RequestServices.GetRequiredService<ISender>();
        var request = context.Arguments[0] ?? await httpContext.BindToAsync(_type);
        // request ??= await httpContext.BindToAsync(_type);
        // if (!httpContext.Items.TryGetValue("Request", out var request) || request is null) {
        //     request = await httpContext.BindToAsync(_type); //.ConfigureAwait(false);
        // }

        return await sender.Send(request, httpContext.RequestAborted).ConfigureAwait(false);
    }
}
