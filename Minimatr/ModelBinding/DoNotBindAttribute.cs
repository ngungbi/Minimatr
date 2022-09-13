namespace Minimatr.ModelBinding;

/// <summary>
/// Disable implicit binding to current property.
/// </summary>
public sealed class DoNotBindAttribute : BindFromAttribute {
    /// <summary>
    /// Disable implicit binding.
    /// </summary>
    public DoNotBindAttribute() : base(null, BindingSource.None) { }

    [Obsolete("This constructor shoul not be used.")]
    public DoNotBindAttribute(string? name) : base(name, BindingSource.None) { }
}
