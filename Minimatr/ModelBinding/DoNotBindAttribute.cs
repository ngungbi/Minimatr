namespace Minimatr.ModelBinding;

public sealed class DoNotBindAttribute : BindFromAttribute {
    public DoNotBindAttribute() : this(null) { }
    public DoNotBindAttribute(string? name) : base(name, BindingSource.None) { }
}
