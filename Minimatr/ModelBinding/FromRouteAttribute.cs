namespace Minimatr.ModelBinding;

/// <summary>
/// Bind value from route path
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class FromRouteAttribute : BindFromAttribute {
    /// <summary>
    /// Bind value from route path
    /// </summary>
    public FromRouteAttribute() : this(null) { }

    /// <summary>
    /// Bind value from route path
    /// </summary>
    /// <param name="name">When null, will use property name</param>
    public FromRouteAttribute(string? name) : base(name, BindingSource.Route) { }
}
