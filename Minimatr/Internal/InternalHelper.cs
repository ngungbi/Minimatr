using System.Reflection;
using Minimatr.ModelBinding;
using Minimatr.RouteHandling.Filter;

namespace Minimatr.Internal;

internal static class InternalHelper {
    internal static IEnumerable<RouteHandlerFilterAttribute> GetFilters(MemberInfo type)
        => type.GetCustomAttributes<RouteHandlerFilterAttribute>().Reverse().ToList();

    internal static IEnumerable<MapMethodAttribute> GetMapMethods(MemberInfo type)
        => type.GetCustomAttributes<MapMethodAttribute>();

    // internal static void BuildModel(Type type)
    //     => ModelBinder.Build(type);
}
