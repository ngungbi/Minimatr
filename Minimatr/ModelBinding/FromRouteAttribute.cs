namespace Minimatr.ModelBinding;

[AttributeUsage(AttributeTargets.Property)]
public sealed class FromRouteAttribute : BindFromAttribute {
    public FromRouteAttribute() : this(null) { }
    public FromRouteAttribute(string? name) : base(name, BindingSource.Route) { }
}
