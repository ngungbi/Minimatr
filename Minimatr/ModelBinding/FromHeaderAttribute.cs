namespace Minimatr.ModelBinding;

[AttributeUsage(AttributeTargets.Property)]
public sealed class FromHeaderAttribute : BindFromAttribute {
    public FromHeaderAttribute(string? name) : base(name, BindingSource.Header) { }

    public FromHeaderAttribute() : this(null) { }
}
