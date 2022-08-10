namespace Minimatr.ModelBinding;

[AttributeUsage(AttributeTargets.Property)]
public class FromFormAttribute : BindFromAttribute {
    public FromFormAttribute(string? name) : base(name, BindingSource.Form) { Name = name; }

    public FromFormAttribute() : this(null) { }
}
