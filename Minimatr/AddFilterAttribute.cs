using Minimatr.RouteHandling.Filter;

namespace Minimatr;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AddFilterAttribute : Attribute, IRouteHandlerFilter {
    private readonly Type _type;

    public AddFilterAttribute(Type type) {
        if (type.IsInterface) {
            throw new ArgumentException("Type must be a class");
        }

        if (!type.GetInterfaces().Contains(typeof(IRouteHandlerFilter))) {
            throw new ArgumentException("Type must implement IRouteHandlerFilter");
        }

        _type = type;
    }

    public ValueTask<object?> InvokeAsync(RouteHandlerInvocationContext context, RouteHandlerFilterDelegate next) {
        var filter = context.HttpContext.RequestServices.GetRequiredService(_type) as IRouteHandlerFilter ?? Activator.CreateInstance(_type) as IRouteHandlerFilter;

        return filter!.InvokeAsync(context, next);
    }
}
