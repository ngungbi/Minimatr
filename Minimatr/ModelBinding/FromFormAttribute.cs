namespace Minimatr.ModelBinding;

// [AttributeUsage(AttributeTargets.Property)]
/// <summary>
/// Bind property value from form content.
/// </summary>
public sealed class FromFormAttribute : BindFromAttribute {
    
    /// <summary>
    /// Bind property value from form content.
    /// </summary>
    /// <param name="name">Name alias. When null, will use name of the property.</param>
    public FromFormAttribute(string? name) : base(name, BindingSource.Form) { Name = name; }

    public FromFormAttribute() : this(null) { }
}
