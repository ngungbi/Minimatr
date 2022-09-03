using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Options;
using Minimatr.Configuration;
using Minimatr.Internal;

namespace Minimatr.ModelBinding;

public static class ModelBinder {
    private static readonly TypeObjectDictionary<RequestParameter> ParameterCache = new();
    private static JsonSerializerOptions? _jsonSerializerOptions;

    internal static RequestParameter? GetRequestParameter(Type type) {
        return ParameterCache.TryGetValue(type, out var parameter)
            ? parameter
            : null;
    }


    // internal static async Task<T> BindToTargetAsync<T>(this HttpContext context, T target) where T : IEndpointRequest {
    //     return (T) await BindToAsync(context, typeof(T), target);
    // }

    internal static async Task<T> BindToAsync<T>(this HttpContext context) where T : IEndpointRequest {
        return (T) await BindToAsync(context, typeof(T));
    }

    internal static Task<object> BindToAsync(this HttpContext context, Type type) {
        var target = Activator.CreateInstance(type);
        if (target is null) throw new InvalidOperationException($"Failed to create instance of type {type.FullName}");
        return BindToAsync(context, type, target);
    }

    internal static async Task<object> BindToAsync(this HttpContext context, Type type, object target) {
        if (!ParameterCache.TryGetValue(type, out var parameters)) {
            Build(type, context, context.RequestServices.GetService<ObjectParserCollection>());
            parameters = ParameterCache[type];
        }

        // var target = Activator.CreateInstance(type);
        // if (target is null) throw new NullReferenceException($"Failed to create instance of type {type.FullName}");
        foreach (var property in parameters.Queries) {
            if (context.Request.Query.TryGetValue(property.Name, out var queryValue))
                property.SetValue(target, queryValue);
        }

        foreach (var property in parameters.Routes) {
            if (context.Request.RouteValues.TryGetValue(property.Name, out var routeValue))
                property.SetValue(target, routeValue as string);
        }

        if (parameters.ExpectFormBody && context.Request.HasFormContentType) {
            foreach (var property in parameters.Forms) {
                if (context.Request.Form.TryGetValue(property.Name, out var formValue))
                    property.SetValue(target, formValue);
            }
        }

        if (parameters.ExpectJsonBody && context.Request.HasJsonContentType()) {
            // var jsonOptions = context.RequestServices.GetService<IOptions<JsonOptions>>();
            _jsonSerializerOptions ??= context.RequestServices.GetService<IOptions<JsonOptions>>()?.Value.SerializerOptions;
            foreach (var item in parameters.Bodies) {
                var body = await context.Request.ReadFromJsonAsync(item.PropertyType, _jsonSerializerOptions);
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
                property.SetValue(target, headerValue);
        }

        parameters.HttpContext?.SetValue(target, context);
        parameters.HttpRequest?.SetValue(target, context.Request);
        parameters.HttpResponse?.SetValue(target, context.Response);

        return target;

        // throw new NotImplementedException();
    }

    private static BindFromAttribute? GetInferredBinding(PropertyInfo propertyInfo, RoutePattern pattern) {
        var propType = propertyInfo.PropertyType;
        BindFromAttribute? bind = null;
        if (propType == typeof(string) || propType.IsValueType || propType.IsPrimitive || propType.IsEnum) {
            // implicit binding declaration
            foreach (var parameter in pattern.Parameters) {
                if (!string.Equals(parameter.Name, propertyInfo.Name, StringComparison.OrdinalIgnoreCase)) continue;
                bind = new FromRouteAttribute(parameter.Name);
                break;
            }

            bind ??= new BindFromAttribute(BindingSource.Query);
        }

        return bind;
    }

    // internal static void Build<T>(ObjectParserCollection? parsers = null) => Build(typeof(T), parsers);
    // internal static void Build(Type type, ObjectParserCollection? parsers = null) => Build(type, null, parsers);

    internal static void Build(Type type, HttpContext? context, ObjectParserCollection? parsers = null) {
        Debug.Assert(parsers != null);
        Debug.Assert(context != null);
        var parameters = new RequestParameter();
        var properties = type.GetProperties();
        var endpoint = context.GetEndpoint() as RouteEndpoint;
        var config = context.RequestServices.GetRequiredService<IOptions<MinimatrConfiguration>>().Value;
        foreach (var property in properties) {
            var bind = property.GetCustomAttribute<BindFromAttribute>();
            if (bind is null) {
                if (!config.EnableInferredBinding) continue;
                bind = GetInferredBinding(property, endpoint?.RoutePattern!);
            } else if (bind.Source == BindingSource.None) {
                continue;
            }

            var propType = property.PropertyType;
            switch (bind?.Source) {
                case BindingSource.Route:
                    parameters.Routes.Add(bind.Name, property, parsers);
                    // parsers.AddToList(bind.Name, parameters.Routes, property);
                    break;
                case BindingSource.Query:
                    parameters.Queries.Add(bind.Name, property, parsers);
                    // parsers.AddToList(bind.Name, parameters.Queries, property);
                    break;
                case BindingSource.Form:
                    parameters.ExpectFormBody = true;
                    parameters.Forms.Add(bind.Name, property, parsers);
                    // parsers.AddToList(bind.Name, parameters.Forms, property);
                    break;
                case BindingSource.Header:
                    parameters.Headers.Add(bind.Name, property, parsers);
                    // parsers.AddToList(bind.Name, parameters.Headers, property);
                    break;
                case BindingSource.None: break;
                case BindingSource.Body: break;
                case BindingSource.File: break;
                case BindingSource.Unknown: break;
                case null: break;
                default: throw new ArgumentOutOfRangeException();
            }


            if (IsHttpContext(property, parameters)) continue;

            // if (property.GetCustomAttribute<FromFormAttribute>() != null) {
            //     // if (parameters.Body is not null) throw new ArgumentException("Multiple request body model found");
            // } else 
            if (property.GetCustomAttribute<FromBodyAttribute>() != null) {
                var propInterfaces = property.PropertyType.GetInterfaces();

                if (propType == typeof(IFormFileCollection) || propInterfaces.Contains(typeof(IFormFileCollection))) {
                    parameters.ExpectFormFileCollection = true;
                    parameters.FormFile = property;
                } else if (propType == typeof(IFormFile) || propInterfaces.Contains(typeof(IFormFile))) {
                    parameters.ExpectFormFile = true;
                    parameters.FormFile = property;
                } else {
                    parameters.ExpectJsonBody = true;
                    parameters.Bodies.Add(property); //.Body = property;
                }
            }
        }

        ParameterCache.Add(type, parameters);
    }

    private static bool IsHttpContext(PropertyInfo property, RequestParameter parameters) {
        var propType = property.PropertyType;
        if (propType == typeof(HttpContext)) {
            parameters.HttpContext = property;
            return true;
        }

        if (propType == typeof(HttpRequest)) {
            parameters.HttpRequest = property;
            return true;
        }

        if (propType == typeof(HttpResponse)) {
            parameters.HttpResponse = property;
            return true;
        }

        return false;
    }

    // private static void AddToList(string? name, ICollection<PropertySetter> list, PropertyInfo property, ObjectParserCollection parsers) {
    //     var baseType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
    //     if (!parsers.TryGetValue(baseType, out var parser)) {
    //         throw new ArgumentException($"No parser found for type {property.PropertyType.FullName}");
    //     }
    //
    //     list.Add(new PropertySetter(name ?? property.Name, new DefaultObjectSetter(property, parser)));
    //     // var parser = parsers?.GetValue(property.PropertyType);
    //     // if (parser is null) return;
    // }
    //
    // internal static bool TryAddProperty<TAttr>(ICollection<PropertySetter> list, PropertyInfo property, ObjectParserCollection? parsers = null) where TAttr : Attribute, IModelNameProvider {
    //     var attr = property.GetCustomAttribute<TAttr>();
    //     if (attr == null) return false;
    //     if (parsers is null) {
    //         list.Add(new PropertySetter(property, attr.Name));
    //         // return true;
    //     } else if (parsers.TryGetValue(property.PropertyType, out var parser)) {
    //         list.Add(new PropertySetter(attr.Name ?? property.Name, new DefaultObjectSetter(property, parser)));
    //         // return true;
    //     } else {
    //         return false;
    //     }
    //
    //     return true;
    // }

    // internal static bool TryAddProperty<TAttr>(ICollection<PropertySetter> list, PropertyInfo property) where TAttr : Attribute, IModelNameProvider {
    //     var attr = property.GetCustomAttribute<TAttr>();
    //     if (attr == null) return false;
    //     list.Add(new PropertySetter(property, attr.Name));
    //     return true;
    // }

    internal static void AddDefaultParsers(ObjectParserCollection collection) {
        collection.TryAdd(typeof(string), input => input.ToString());
        collection.TryAdd(typeof(string[]), input => input.ToArray());
        collection.TryAdd(typeof(IEnumerable<string>), input => input.ToArray());
        // collection.TryAdd(typeof(int), input => int.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(long), input => long.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(bool), input => bool.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(double), input => double.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(float), input => float.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(decimal), input => decimal.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(DateTime), input => DateTime.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(TimeSpan), input => TimeSpan.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(Guid), input => Guid.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(byte), input => byte.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(sbyte), input => sbyte.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(short), input => short.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(ushort), input => ushort.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(uint), input => uint.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(ulong), input => ulong.TryParse(input, out var result) ? result : default);
        // collection.TryAdd(typeof(char), input => char.TryParse(input, out var result) ? result : default);
    }
}
