namespace Minimatr.ModelBinding;

[AttributeUsage(AttributeTargets.Property)]
public sealed class FromQueryAttribute : BindFromAttribute {
    public FromQueryAttribute() : this(null) { }
    public FromQueryAttribute(string? name) : base(name, BindingSource.Query) { }
}
