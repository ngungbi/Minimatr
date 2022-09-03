using System.Reflection;
using Minimatr.ModelBinding;

namespace Minimatr.Internal;

internal class RequestParameter {
    // internal bool HasQueryParameters { get; set; }
    // internal bool HasRouteParameters { get; set; }
    // internal bool HasHeaderParameters { get; set; }
    internal bool ExpectFormFile { get; set; }
    internal bool ExpectFormFileCollection { get; set; }
    internal bool ExpectFormBody { get; set; }
    internal bool ExpectJsonBody { get; set; }

    internal PropertySetterCollection Routes { get; } = new();
    internal PropertySetterCollection Queries { get; } = new();
    internal PropertySetterCollection Headers { get; } = new();

    internal PropertySetterCollection Forms { get; } = new();

    // internal List<PropertyInfo> Files { get; set; } = new();
    internal List<PropertyInfo> Bodies { get; set; } = new();

    // internal PropertyInfo? Body { get; set; }
    internal PropertyInfo? FormFile { get; set; }
    internal PropertyInfo? FormFileCollection { get; set; }
    internal PropertyInfo? HttpContext { get; set; }
    internal PropertyInfo? HttpRequest { get; set; }
    internal PropertyInfo? HttpResponse { get; set; }

    // public object Body { get; init; } = default!;
}
