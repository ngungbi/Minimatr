using System.Reflection;
using Microsoft.Extensions.Primitives;

namespace Minimatr.ModelBinding;

internal delegate void PropertySetterDelegate(object obj, object? value);

public delegate object ObjectParserDelegate(StringValues input);

internal delegate bool GenericSetterDelegate<T>(string value, out T result);

internal abstract class ObjectSetter {
    protected readonly PropertySetterDelegate Setter;
    // protected readonly MethodInfo SetMethodInfo;

    public ObjectSetter(PropertyInfo propertyInfo) {
        // SetMethodInfo = propertyInfo.SetValue();

        Setter = propertyInfo.SetValue;
    }

    public abstract void SetValue(object obj, StringValues value);

    protected static void ThrowInvalidCast(StringValues value, Type type)
        => throw new InvalidCastException($"Unable to convert '{value}' to '{type.FullName}' type");
}

// internal delegate T ObjectParserDelegate<out T>(StringValues input);

internal sealed class DefaultObjectSetter : ObjectSetter {
    private readonly ObjectParserDelegate _parser;
    public DefaultObjectSetter(PropertyInfo propertyInfo, ObjectParserDelegate parser) : base(propertyInfo) { _parser = parser; }

    public override void SetValue(object obj, StringValues value) {
        var result = _parser(value);
        Setter(obj, result);
    }
}

internal sealed class StringSetter : ObjectSetter {
    public StringSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) => Setter(obj, value.ToString());
}

internal sealed class StringsSetter : ObjectSetter {
    public StringsSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) => Setter(obj, value.ToArray());
}

internal sealed class EnumSetter : ObjectSetter {
    private readonly Type _type;
    public EnumSetter(PropertyInfo propertyInfo, Type type) : base(propertyInfo) { _type = type; }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count == 0) return;
        if (Enum.TryParse(_type, value[0], true, out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
        ThrowInvalidCast(value, _type);
    }
}

internal sealed class ObjectSetter<T> : ObjectSetter {
    private readonly GenericSetterDelegate<T> _parser;

    public ObjectSetter(PropertyInfo propertyInfo, GenericSetterDelegate<T> parser) : base(propertyInfo) {
        _parser = parser;
    }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count == 0) return;
        var rawValue = value[0];
        var success = _parser(rawValue, out var result);
        if (!success) ThrowInvalidCast(value, typeof(T));
        // SetMethodInfo.Invoke(obj, new object?[] {result});
        Setter(obj, result);
    }
}

internal sealed class ParseableSetter : ObjectSetter {
    private readonly MethodInfo _parser;

    public ParseableSetter(PropertyInfo propertyInfo, MethodInfo parser) : base(propertyInfo) {
        _parser = parser;
    }

    public override void SetValue(object obj, StringValues value) {
        if (obj is IParseable parsed && parsed.TryParse(value[0], out var result)) {
            // PropertyInfo.SetValue(obj, result);
            Setter(obj, result);
            return;
        }

        var valueAsString = value.ToString();
        object?[] args = {valueAsString, null};
        if (valueAsString is not null && _parser.Invoke(null, args) is true) {
            // PropertyInfo.SetValue(obj, args[1]);
            Setter(obj, args[1]);
        }
    }
}

// internal class TimeSpanSetter : ObjectSetter {
//     public TimeSpanSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (TimeSpan.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(TimeSpan));
//     }
// }
//
// internal class GuidSetter : ObjectSetter {
//     public GuidSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (Guid.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(Guid));
//     }
// }
//
// internal class Int16Setter : ObjectSetter {
//     public Int16Setter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (short.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(short));
//     }
// }
//
// // public class SingleSetter : ObjectSetter {
// //     public SingleSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }
// //
// //     public override void SetValue(object obj, StringValues value) {
// //         if (value.Count == 0) return;
// //         if (float.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
// //         ThrowInvalidCast(value, typeof(float));
// //     }
// // }
//
// internal class CharSetter : ObjectSetter {
//     public CharSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (char.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(char));
//     }
// }
//
// internal class Int32Setter : ObjectSetter {
//     public Int32Setter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (int.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(int));
//     }
// }
//
// internal class BooleanSetter : ObjectSetter {
//     public BooleanSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (bool.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         if (short.TryParse(value[0], out var shortValue)) Setter(obj, shortValue != 0);
//         ThrowInvalidCast(value, typeof(bool));
//     }
// }
//
// internal class DateTimeSetter : ObjectSetter {
//     public DateTimeSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (DateTime.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(DateTime));
//     }
// }
//
// internal class DecimalSetter : ObjectSetter {
//     public DecimalSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (decimal.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(decimal));
//     }
// }
//
// internal class DoubleSetter : ObjectSetter {
//     public DoubleSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (double.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(double));
//     }
// }
//
// internal class FloatSetter : ObjectSetter {
//     public FloatSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (float.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(float));
//     }
// }
//
// internal class Int64Setter : ObjectSetter {
//     public Int64Setter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (long.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(long));
//     }
// }
//
// internal class UInt64Setter : ObjectSetter {
//     public UInt64Setter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (ulong.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(ulong));
//     }
// }
//
// internal class UInt32Setter : ObjectSetter {
//     public UInt32Setter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (uint.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(uint));
//     }
// }
//
// internal class UInt16Setter : ObjectSetter {
//     public UInt16Setter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (ushort.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(ushort));
//     }
// }
//
// internal class ByteSetter : ObjectSetter {
//     public ByteSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (byte.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(byte));
//     }
// }
//
// internal class SByteSetter : ObjectSetter {
//     public SByteSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }
//
//     public override void SetValue(object obj, StringValues value) {
//         if (value.Count == 0) return;
//         if (sbyte.TryParse(value[0], out var result)) Setter(obj, result); // PropertyInfo.SetValue(obj, result);
//         ThrowInvalidCast(value, typeof(sbyte));
//     }
// }
