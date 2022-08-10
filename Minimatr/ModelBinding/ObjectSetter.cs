using System.Reflection;
using Microsoft.Extensions.Primitives;

namespace Minimatr.ModelBinding;

public abstract class ObjectSetter {
    protected readonly PropertyInfo PropertyInfo;
    public ObjectSetter(PropertyInfo propertyInfo) { PropertyInfo = propertyInfo; }

    public abstract void SetValue(object obj, StringValues value);
}

public delegate object ObjectParserDelegate(StringValues input);

public delegate T ObjectParserDelegate<out T>(StringValues input);

public class DefaultObjectSetter : ObjectSetter {
    private readonly ObjectParserDelegate _parser;
    public DefaultObjectSetter(PropertyInfo propertyInfo, ObjectParserDelegate parser) : base(propertyInfo) { _parser = parser; }

    public override void SetValue(object obj, StringValues value) {
        var result = _parser(value);
        PropertyInfo.SetValue(obj, result);
    }
}

public class StringSetter : ObjectSetter {
    public StringSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        PropertyInfo.SetValue(obj, value.ToString());
    }
}

public class StringsSetter : ObjectSetter {
    public StringsSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        PropertyInfo.SetValue(obj, value.ToArray());
    }
}

public class EnumSetter : ObjectSetter {
    private readonly Type _type;
    public EnumSetter(PropertyInfo propertyInfo, Type type) : base(propertyInfo) { _type = type; }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && Enum.TryParse(_type, value[0], true, out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class ParseableSetter : ObjectSetter {
    private readonly MethodInfo _parser;

    public ParseableSetter(PropertyInfo propertyInfo, MethodInfo parser) : base(propertyInfo) {
        _parser = parser;
    }

    public override void SetValue(object obj, StringValues value) {
        if (obj is IParseable parsed && parsed.TryParse(value[0], out var result)) {
            PropertyInfo.SetValue(obj, result);
            return;
        }

        var valueAsString = value.ToString();
        object?[] args = {valueAsString, null};
        if (valueAsString is not null && _parser.Invoke(null, args) is true) {
            PropertyInfo.SetValue(obj, args[1]);
        }
    }
}

public class TimeSpanSetter : ObjectSetter {
    public TimeSpanSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && TimeSpan.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class GuidSetter : ObjectSetter {
    public GuidSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && Guid.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class Int16Setter : ObjectSetter {
    public Int16Setter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && short.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class SingleSetter : ObjectSetter {
    public SingleSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && float.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class CharSetter : ObjectSetter {
    public CharSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && char.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class Int32Setter : ObjectSetter {
    public Int32Setter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && int.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class BooleanSetter : ObjectSetter {
    public BooleanSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && bool.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class DateTimeSetter : ObjectSetter {
    public DateTimeSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && DateTime.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class DecimalSetter : ObjectSetter {
    public DecimalSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && decimal.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class DoubleSetter : ObjectSetter {
    public DoubleSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && double.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class FloatSetter : ObjectSetter {
    public FloatSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && float.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class Int64Setter : ObjectSetter {
    public Int64Setter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && long.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class UInt64Setter : ObjectSetter {
    public UInt64Setter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && ulong.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class UInt32Setter : ObjectSetter {
    public UInt32Setter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && uint.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class UInt16Setter : ObjectSetter {
    public UInt16Setter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && ushort.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class ByteSetter : ObjectSetter {
    public ByteSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && byte.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}

public class SByteSetter : ObjectSetter {
    public SByteSetter(PropertyInfo propertyInfo) : base(propertyInfo) { }

    public override void SetValue(object obj, StringValues value) {
        if (value.Count > 0 && sbyte.TryParse(value[0], out var result)) PropertyInfo.SetValue(obj, result);
    }
}
