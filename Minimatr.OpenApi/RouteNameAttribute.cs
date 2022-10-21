namespace Minimatr.OpenApi;

public sealed class RouteNameAttribute : Attribute {
    public string? Name { get; }

    public RouteNameAttribute(string? name) { Name = name; }
}
