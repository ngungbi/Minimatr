namespace Minimatr.ModelBinding;

/// <summary>
/// Bind value from query string.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class FromQueryAttribute : BindFromAttribute {
    /// <summary>
    /// Bind value from query string
    /// </summary>
    public FromQueryAttribute() : this(null) { }

    /// <summary>
    /// Bind value from query string
    /// </summary>
    /// <param name="name">Name alias. When null, will use the name of property.</param>
    public FromQueryAttribute(string? name) : base(name, BindingSource.Query) { }
}
