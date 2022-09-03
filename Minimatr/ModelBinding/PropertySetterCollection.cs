using System.Reflection;

namespace Minimatr.ModelBinding;

internal class PropertySetterCollection : List<PropertySetter> {
    public void Add(string? name, PropertyInfo property, ObjectParserCollection parsers) {
        var baseType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        if (parsers.TryGetValue(baseType, out var parser)) {
            Add(new PropertySetter(name ?? property.Name, new DefaultObjectSetter(property, parser)));
            return;
        }

        Add(new PropertySetter(property, name));
        // throw new ArgumentException($"No parser found for type {property.PropertyType.FullName}");
    }
}
