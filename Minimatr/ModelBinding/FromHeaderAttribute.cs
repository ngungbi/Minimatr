namespace Minimatr.ModelBinding;

[AttributeUsage(AttributeTargets.Property)]
public class FromHeaderAttribute : BindFromAttribute {
    public FromHeaderAttribute(string? name) : base(name, BindingSource.Header) { }

    public FromHeaderAttribute() : this(null) { }
}
