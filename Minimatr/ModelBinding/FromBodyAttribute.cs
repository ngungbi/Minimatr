namespace Minimatr.ModelBinding;

// [AttributeUsage(AttributeTargets.Property)]
/// <summary>
/// Bind property value from request body. Only support JSON content.
/// </summary>
public sealed class FromBodyAttribute : BindFromAttribute {
    /// <summary>
    /// Bind property value from request body.
    /// </summary>
    public FromBodyAttribute() : base(null, BindingSource.Body) { }

    /// <summary>
    /// Bind property value from request body.
    /// </summary>
    /// <param name="name">Name alias. When null, will use name of the property.</param>
    [Obsolete("Name parameter will not be used.")]
    public FromBodyAttribute(string? name) : base(name, BindingSource.Body) { }
}
