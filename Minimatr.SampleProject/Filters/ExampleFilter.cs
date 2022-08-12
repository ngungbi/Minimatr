using Minimatr.RouteHandling.Filter;

namespace Minimatr.SampleProject.Filters; 

public class ExampleFilter:IRouteHandlerFilter {
    public ValueTask<object?> InvokeAsync(RouteHandlerInvocationContext context, RouteHandlerFilterDelegate next) {
        throw new NotImplementedException();
    }
}
