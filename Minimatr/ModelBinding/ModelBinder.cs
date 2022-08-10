using System.Reflection;
using Microsoft.AspNetCore.Http;
using Minimatr.Internal;

namespace Minimatr.ModelBinding;

public static class ModelBinder {
    private static readonly TypeObjectDictionary<RequestParameter> ParameterCache = new();

    internal static RequestParameter? GetRequestParameter(Type type) {
        return ParameterCache.TryGetValue(type, out var parameter)
            ? parameter
            : null;
    }

    internal static async Task<T> BindTo<T>(this HttpContext context) where T : IEndpointRequest {
        return (T) await BindTo(context, typeof(T));
    }

    internal static async Task<object> BindTo(this HttpContext context, Type type) {
        if (!ParameterCache.TryGetValue(type, out var parameters)) {
            Build(type, context.RequestServices.GetService<ObjectParserCollection>());
            parameters = ParameterCache[type];
        }

        var target = Activator.CreateInstance(type);
        if (target is null) throw new NullReferenceException($"Failed to create instance of type {type.FullName}");
        foreach (var property in parameters.Queries) {
            if (context.Request.Query.TryGetValue(property.Name, out var queryValue))
                property.Setter.SetValue(target, queryValue);
        }

        foreach (var property in parameters.Routes) {
            if (context.Request.RouteValues.TryGetValue(property.Name, out var routeValue))
                property.Setter.SetValue(target, routeValue as string);
        }

        if (parameters.ExpectFormBody && context.Request.HasFormContentType) {
            foreach (var property in parameters.Forms) {
                if (context.Request.Form.TryGetValue(property.Name, out var formValue))
                    property.Setter.SetValue(target, formValue);
            }
        }

        if (parameters.ExpectJsonBody && context.Request.HasJsonContentType()) {
            foreach (var item in parameters.Bodies) {
                var body = await context.Request.ReadFromJsonAsync(item.PropertyType);
                item.SetValue(target, body);
            }
            // context.Request.Form.Files
        }

        if (parameters.ExpectFormFileCollection) {
            parameters.FormFileCollection?.SetValue(target, context.Request.Form.Files);
        }

        if (parameters.ExpectFormFile && context.Request.Form.Files.Count > 0) {
            // assumes only has 1 property that expect file input
            parameters.FormFile?.SetValue(target, context.Request.Form.Files[0]);
        }

        foreach (var property in parameters.Headers) {
            if (context.Request.Headers.TryGetValue(property.Name, out var headerValue))
                property.Setter.SetValue(target, headerValue);
        }

        parameters.HttpContext?.SetValue(target, context);
        parameters.HttpRequest?.SetValue(target, context.Request);
        parameters.HttpResponse?.SetValue(target, context.Response);

        return target;

        // throw new NotImplementedException();
    }

    internal static void Build<T>(ObjectParserCollection? parsers = null) => Build(typeof(T), parsers);

    internal static void Build(Type type, ObjectParserCollection? parsers = null) {
        var parameters = new RequestParameter();
        var properties = type.GetProperties();
        foreach (var property in properties) {
            if (TryAddProperty<FromQueryAttribute>(parameters.Queries, property, parsers)) {
                // parameters.HasQueryParameters = true;
                continue;
            }

            if (TryAddProperty<FromRouteAttribute>(parameters.Routes, property, parsers)) {
                // parameters.HasRouteParameters = true;
                continue;
            }

            if (TryAddProperty<FromFormAttribute>(parameters.Forms, property, parsers)) {
                // parameters.ExpectFormBody = true;
                continue;
            }

            if (TryAddProperty<FromHeaderAttribute>(parameters.Headers, property, parsers)) {
                // parameters.HasHeaderParameters = true;
                continue;
            }

            var propType = property.PropertyType;

            if (propType == typeof(HttpContext)) {
                parameters.HttpContext = property;
                continue;
            }

            if (propType == typeof(HttpRequest)) {
                parameters.HttpRequest = property;
                continue;
            }

            if (propType == typeof(HttpResponse)) {
                parameters.HttpResponse = property;
                continue;
            }

            if (property.GetCustomAttribute<FromBodyAttribute>() != null) { //propType.IsClass || (propType.IsValueType && !propType.IsPrimitive && !propType.IsEnum)) {
                // if (parameters.Body is not null) throw new ArgumentException("Multiple request body model found");
                var propInterfaces = property.PropertyType.GetInterfaces();

                if (propInterfaces.Contains(typeof(IFormFileCollection))) {
                    parameters.ExpectFormFileCollection = true;
                    parameters.FormFile = property;
                    // parameters.Files.Add(property);
                    continue;
                }

                if (propInterfaces.Contains(typeof(IFormFile))) {
                    parameters.ExpectFormFile = true;
                    parameters.FormFile = property;
                    // parameters.Files.Add(property);
                    continue;
                }

                parameters.Bodies.Add(property); //.Body = property;
            }
        }

        ParameterCache.Add(type, parameters);
    }

    internal static bool TryAddProperty<TAttr>(ICollection<PropertySetter> list, PropertyInfo property, ObjectParserCollection? parsers = null) where TAttr : Attribute, IModelNameProvider {
        var attr = property.GetCustomAttribute<TAttr>();
        if (attr == null) return false;
        if (parsers is null) {
            list.Add(new PropertySetter(property, attr.Name));
            // return true;
        } else if (parsers.TryGetValue(property.PropertyType, out var parser)) {
            list.Add(new PropertySetter(attr.Name ?? property.Name, new DefaultObjectSetter(property, parser)));
            // return true;
        } else {
            return false;
        }

        return true;
    }

    // internal static bool TryAddProperty<TAttr>(ICollection<PropertySetter> list, PropertyInfo property) where TAttr : Attribute, IModelNameProvider {
    //     var attr = property.GetCustomAttribute<TAttr>();
    //     if (attr == null) return false;
    //     list.Add(new PropertySetter(property, attr.Name));
    //     return true;
    // }

    internal static void AddDefaultParsers(ObjectParserCollection collection) {
        collection.TryAdd(typeof(int), input => int.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(long), input => long.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(bool), input => bool.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(double), input => double.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(float), input => float.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(decimal), input => decimal.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(DateTime), input => DateTime.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(TimeSpan), input => TimeSpan.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(Guid), input => Guid.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(byte), input => byte.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(sbyte), input => sbyte.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(short), input => short.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(ushort), input => ushort.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(uint), input => uint.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(ulong), input => ulong.TryParse(input, out var result) ? result : default);
        collection.TryAdd(typeof(char), input => char.TryParse(input, out var result) ? result : default);
    }
}
