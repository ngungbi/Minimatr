namespace Minimatr.ModelBinding;

[AttributeUsage(AttributeTargets.Property)]
public class FromQueryAttribute : BindFromAttribute {
    public FromQueryAttribute() : this(null) { }
    public FromQueryAttribute(string? name) : base(name, BindingSource.Query) { }
}
