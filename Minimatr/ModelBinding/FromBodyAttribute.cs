namespace Minimatr.ModelBinding;

[AttributeUsage(AttributeTargets.Property)]
public sealed class FromBodyAttribute : BindFromAttribute {
    public FromBodyAttribute() : this(null) { }
    public FromBodyAttribute(string? name) : base(name, BindingSource.Body) { }
}
