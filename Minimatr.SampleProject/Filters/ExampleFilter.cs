using Minimatr.RouteHandling.Filter;

namespace Minimatr.SampleProject.Filters;

public class ExampleFilter : IRouteHandlerFilter {
    public async ValueTask<object?> InvokeAsync(RouteHandlerInvocationContext context, RouteHandlerFilterDelegate next) {
        Console.WriteLine("Filter here");
        return await next(context);
    }
}
