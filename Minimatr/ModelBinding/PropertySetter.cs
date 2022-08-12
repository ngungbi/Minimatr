using System.Reflection;
using Microsoft.Extensions.Primitives;

namespace Minimatr.ModelBinding;

internal class PropertySetter {
    public string Name { get; set; }

    // public Action<object, StringValues> SetValue { get; set; }
    public ObjectSetter Setter { get; set; }


    public PropertySetter(PropertyInfo propertyInfo, string? name = null) {
        Name = name ?? propertyInfo.Name;

        if (IsStringEnumerable(propertyInfo.PropertyType)) {
            Setter = new StringsSetter(propertyInfo);
            return;
        }

        var type = GetBaseType(propertyInfo.PropertyType);
        Setter = GetSetter(type, propertyInfo);
    }

    public PropertySetter(string name, ObjectSetter setter) {
        Name = name;
        Setter = setter;
    }


    private static ObjectSetter GetSetter(Type type, PropertyInfo propertyInfo) {
        if (type == typeof(string)) {
            return new StringSetter(propertyInfo); // (obj, value) => propertyInfo.SetValue(obj, value.ToString());
        }

        // if (type.IsArray || IsEnumerable(type)) {
        //     var baseType = type.GetGenericArguments()[0];
        //     var setter = GetSetter(baseType, propertyInfo); 
        // }

        if (type.IsEnum) return new EnumSetter(propertyInfo, type); // (obj, value) => EnumSetter(obj, value, type, propertyInfo);

        if (type == typeof(int)) return new Int32Setter(propertyInfo); //(obj, value) => Int32Setter(obj, value, propertyInfo);
        if (type == typeof(long)) return new Int64Setter(propertyInfo); //(obj, value) => Int64Setter(obj, value, propertyInfo);
        if (type == typeof(bool)) return new BooleanSetter(propertyInfo); //(obj, value) => BooleanSetter(obj, value, propertyInfo);
        if (type == typeof(double)) return new DoubleSetter(propertyInfo); //(obj, value) => DoubleSetter(obj, value, propertyInfo);
        if (type == typeof(float)) return new FloatSetter(propertyInfo); //(obj, value) => SingleSetter(obj, value, propertyInfo);
        if (type == typeof(decimal)) return new DecimalSetter(propertyInfo); //(obj, value) => DecimalSetter(obj, value, propertyInfo);
        if (type == typeof(DateTime)) return new DateTimeSetter(propertyInfo); //(obj, value) => DateTimeSetter(obj, value, propertyInfo);
        if (type == typeof(TimeSpan)) return new TimeSpanSetter(propertyInfo); //(obj, value) => TimeSpanSetter(obj, value, propertyInfo);
        if (type == typeof(Guid)) return new GuidSetter(propertyInfo); //(obj, value) => GuidSetter(obj, value, propertyInfo);
        if (type == typeof(byte)) return new ByteSetter(propertyInfo); //(obj, value) => ByteSetter(obj, value, propertyInfo);
        if (type == typeof(sbyte)) return new SByteSetter(propertyInfo); //(obj, value) => SByteSetter(obj, value, propertyInfo);
        if (type == typeof(short)) return new Int16Setter(propertyInfo); //(obj, value) => Int16Setter(obj, value, propertyInfo);
        if (type == typeof(ushort)) return new UInt16Setter(propertyInfo); //(obj, value) => UInt16Setter(obj, value, propertyInfo);
        if (type == typeof(uint)) return new UInt32Setter(propertyInfo); //(obj, value) => UInt32Setter(obj, value, propertyInfo);
        if (type == typeof(ulong)) return new UInt64Setter(propertyInfo); //(obj, value) => UInt64Setter(obj, value, propertyInfo);
        if (type == typeof(char)) return new CharSetter(propertyInfo); //(obj, value) => CharSetter(obj, value, propertyInfo);

        var parser = GetParser(type);
        if (parser is null) {
            throw new NotSupportedException($"Type not supported: {type.Name}");
            // return new StringSetter(propertyInfo); // (_, _) => { };
        }

        return new ParseableSetter(propertyInfo, parser); // (obj, value) => ParseableSetter(obj, value.FirstOrDefault(), propertyInfo, parser);
    }

    private static MethodInfo? GetParser(Type type) {
        var paramTypes = new[] {typeof(string), type.MakeByRefType()};
        return type.GetMethod("TryParse", paramTypes);
    }

    private static void ArraySetter(object obj, StringValues values, Action<object, StringValues> setter) {
        foreach (string item in values) {
            setter(obj, item);
        }
    }

    private static void EnumSetter(object obj, string? value, Type type, PropertyInfo propertyInfo) {
        if (value is not null && Enum.TryParse(type, value, true, out var result)) propertyInfo.SetValue(obj, result);
    }

    private static void ParseableSetter(object obj, string? value, PropertyInfo propertyInfo, MethodBase parser) {
        object?[] args = {value, null};
        if (value is not null && parser.Invoke(null, args) is true) {
            propertyInfo.SetValue(obj, args[1]);
        }
    }

    private static bool IsStringEnumerable(Type type) {
        return type == typeof(string[]) || type == typeof(StringValues) || type == typeof(IEnumerable<string>);
    }

    private static bool IsNullable(Type type) {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    private static bool IsEnumerable(Type type) {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
    }

    private static Type GetBaseType(Type type) {
        // var baseType = IsNullable(type) ? Nullable.GetUnderlyingType(type) : type;// new NullableConverter(type).UnderlyingType : type;

        return Nullable.GetUnderlyingType(type) ?? type; // baseType;
    }

    // private static void Int32Setter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && int.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void Int16Setter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && short.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void Int64Setter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && long.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void UInt16Setter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && ushort.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void UInt32Setter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && uint.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void UInt64Setter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && ulong.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void DoubleSetter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && double.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void SingleSetter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && float.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void DecimalSetter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && decimal.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void BooleanSetter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && bool.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void DateTimeSetter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && DateTime.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void GuidSetter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && Guid.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void TimeSpanSetter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && TimeSpan.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void UriSetter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && Uri.TryCreate(value[0], UriKind.RelativeOrAbsolute, out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void ByteSetter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && byte.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void SByteSetter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && sbyte.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
    //
    // private static void CharSetter(object obj, StringValues value, PropertyInfo propertyInfo) {
    //     if (value.Count > 0 && char.TryParse(value[0], out var result)) propertyInfo.SetValue(obj, result);
    // }
}
