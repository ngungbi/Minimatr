using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Minimatr.Internal;
using Minimatr.ModelBinding;
using FromBodyAttribute = Minimatr.ModelBinding.FromBodyAttribute;
using FromFormAttribute = Minimatr.ModelBinding.FromFormAttribute;
using FromQueryAttribute = Minimatr.ModelBinding.FromQueryAttribute;
using FromRouteAttribute = Minimatr.ModelBinding.FromRouteAttribute;

namespace Minimatr.OpenApi;

public class OpenApiGenerator {
    private readonly Assembly _assembly;
    private readonly XmlDocument _document;

    public OpenApiGenerator(Assembly assembly) {
        _assembly = assembly;
        _document = new XmlDocument();
        var xmlPath = Path.Combine(AppContext.BaseDirectory, assembly.GetName().Name + ".xml");
        if (File.Exists(xmlPath))
            _document.Load(xmlPath);
        else
            _document.LoadXml("<doc></doc>");
    }

    private string? GetSummary(MemberInfo memberInfo) {
        var name = $"P:{memberInfo.ReflectedType!.FullName}.{memberInfo.Name}";
        return GetInnerText(name, "summary");
    }

    private string? GetSummary(Type type) {
        var name = "T:" + type.FullName;
        return GetInnerText(name, "summary");
    }

    private string? GetRemarks(MemberInfo memberInfo) {
        var name = $"P:{memberInfo.ReflectedType!.FullName}.{memberInfo.Name}";
        return GetInnerText(name, "remarks");
    }

    private string? GetExample(MemberInfo memberInfo) {
        var name = $"P:{memberInfo.ReflectedType!.FullName}.{memberInfo.Name}";
        return GetInnerText(name, "example");
    }

    private string? GetRemarks(Type type) {
        var name = "T:" + type.FullName;
        return GetInnerText(name, "remarks");
    }

    private string? GetInnerText(string fullName, string type) {
        var members = _document.GetElementsByTagName("member");
        foreach (XmlElement member in members) {
            var name = member.GetAttribute("name");
            if (name != fullName) continue;
            var summary = member.SelectSingleNode(type)?.InnerText;
            return summary?.Trim();
        }

        return null;
    }

    private static OperationType GetOperationType(string method) {
        return method switch {
            "GET" => OperationType.Get,
            "POST" => OperationType.Post,
            "PUT" => OperationType.Put,
            "DELETE" => OperationType.Delete,
            "PATCH" => OperationType.Patch,
            _ => throw new ArgumentException($"Unknown method {method}")
        };
    }

    private readonly TypeObjectDictionary<OpenApiSchema> _schema = new();
    public IDictionary<Type, OpenApiSchema> Schemas => _schema;

    private string _generatedDocument = string.Empty;

    public async Task<string> GenerateDocument(SchemaGeneratorOptions options) {
        if (!string.IsNullOrEmpty(_generatedDocument)) return _generatedDocument;
        var components = new OpenApiComponents();
        foreach (var scheme in options.SecuritySchemes) {
            components.SecuritySchemes.Add(scheme);
        }

        // var reference = new OpenApiReference();
        var document = new OpenApiDocument {
            Info = options.Info,
            Paths = GeneratePaths(options),
            Components = components,
        };
        foreach (var item in options.SecurityRequirements) {
            document.SecurityRequirements.Add(item);
        }

        if (options.SecuritySchemes.Count > 0) {
            document.SecurityRequirements.Add(
                new OpenApiSecurityRequirement {
                    {options.SecuritySchemes.Values.First(), options.SecuritySchemes.Keys.ToList()}
                }
            );
        }

        foreach (var (key, value) in _schema) {
            components.Schemas.Add(key.Name, value);
        }

        await using var textWriter = new StringWriter(CultureInfo.InvariantCulture);
        var jsonWriter = new OpenApiJsonWriter(textWriter);
        document.SerializeAsV3(jsonWriter);
        var result = textWriter.ToString();
        _generatedDocument = result;
        return result;
    }

    private OpenApiResponses GetResponseTypes(MemberInfo type) {
        var responses = new OpenApiResponses();
        var responseTypes = type.GetCustomAttributes<ProducesResponseTypeAttribute>();
        foreach (var t in responseTypes) {
            if (!_schema.TryGetValue(t.Type, out var schema)) {
                _schema.TryAdd(t.Type, CreateSchema(t.Type));
                schema = _schema[t.Type];
            }

            responses.Add(
                t.StatusCode.ToString(), new OpenApiResponse {
                    Content = {
                        ["application/json"] = new OpenApiMediaType {Schema = schema}
                    }
                }
            );
        }

        if (responses.Count == 0) responses.Add("200", new OpenApiResponse());
        return responses;
    }

    private static string GetTagName(Type type, SchemaGeneratorOptions options) {
        if (options.GroupNameConvention is not null) {
            return options.GroupNameConvention(type, options);
        }

        var dot = type.Namespace?.IndexOf('.') ?? -1;
        return dot == -1 ? type.Name : type.Namespace![(dot + 1)..];
    }

    private static string BuildRoute(RoutePattern pattern) {
        var route = new StringBuilder();
        foreach (var item in pattern.PathSegments) {
            route.Append('/');
            Debug.Assert(item.Parts.Count > 0);
            var part = item.Parts[0];
            if (part.IsParameter && part is RoutePatternParameterPart parameterPart) {
                route.Append('{').Append(parameterPart.Name).Append('}');
            } else if (part.IsLiteral && part is RoutePatternLiteralPart literalPart) {
                route.Append(literalPart.Content);
            }
        }

        return route.ToString();
    }

    public OpenApiPaths GeneratePaths(SchemaGeneratorOptions options) {
        var types = _assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IEndpointRequest))).ToList();
        var paths = new OpenApiPaths();
        foreach (var type in types) {
            var settings = type.GetCustomAttribute<ApiExplorerSettingsAttribute>();
            if (settings?.IgnoreApi ?? false) continue;
            var mapAttr = type.GetCustomAttributes<MapMethodAttribute>();
            var responseTypes = GetResponseTypes(type);
            foreach (var item in mapAttr) {
                // var route = item.Template.StartsWith('/') ? item.Template : '/' + item.Template;
                var pattern = RoutePatternFactory.Parse(item.Template);
                var route = BuildRoute(pattern);
                if (!paths.ContainsKey(route)) {
                    paths.Add(route, new OpenApiPathItem());
                }

                var operation = new OpenApiOperation {
                    Tags = {
                        new OpenApiTag {
                            Name = GetTagName(type, options), // type.Name,
                        }
                    },
                    Summary = GetSummary(type),
                    Description = GetRemarks(type),
                    Responses = responseTypes
                };
                foreach (var security in options.SecurityRequirements) {
                    operation.Security.Add(security);
                }

                var operationType = GetOperationType(item.SupportedMethods.First());
                var hasBody = operationType is OperationType.Post or OperationType.Put or OperationType.Patch;

                var props = type.GetProperties();
                var routeParameters = new List<OpenApiParameter>();
                var queryParameters = new List<OpenApiParameter>();
                var headerParameters = new List<OpenApiParameter>();

                foreach (var propertyInfo in props) {
                    var bind = propertyInfo.GetCustomAttribute<BindFromAttribute>();
                    var propType = propertyInfo.PropertyType;
                    if (bind is null) {
                        bind = GetInferredBinding(propertyInfo, pattern);
                    } else if (bind.Source == BindingSource.None) continue;


                    switch (bind?.Source) {
                        case BindingSource.Query:
                            queryParameters.Add(
                                new OpenApiParameter {
                                    Name = bind.Name ?? propertyInfo.Name, In = ParameterLocation.Query,
                                    Description = GetSummary(propertyInfo),
                                    AllowEmptyValue = true,
                                    Example = new OpenApiString(GetExample(propertyInfo))
                                }
                            );
                            break;
                        case BindingSource.Route:
                            routeParameters.Add(
                                new OpenApiParameter {
                                    Name = bind.Name ?? propertyInfo.Name, In = ParameterLocation.Path,
                                    Required = true,
                                    Description = GetSummary(propertyInfo),
                                    AllowEmptyValue = false,
                                    Example = new OpenApiString(GetExample(propertyInfo))
                                }
                            );
                            break;
                        case BindingSource.Header:
                            headerParameters.Add(
                                new OpenApiParameter {
                                    Name = bind.Name ?? propertyInfo.Name, In = ParameterLocation.Header,
                                    Description = GetSummary(propertyInfo),
                                    AllowEmptyValue = true,
                                    Example = new OpenApiString(GetExample(propertyInfo))
                                }
                            );
                            break;
                        case BindingSource.Body:
                            if (!hasBody || !propType.IsClass || IsHttpContext(propType)) continue;
                            if (propType.GetInterfaces().Contains(typeof(IFormFile))) { }

                            if (!_schema.TryGetValue(propType, out var schema)) {
                                schema = CreateSchema(propType);
                                _schema.Add(propType, schema);
                            }

                            operation.RequestBody = new OpenApiRequestBody {
                                Content = {{"application/json", new OpenApiMediaType {Schema = schema}}},
                                Description = GetSummary(propertyInfo),
                            };
                            var example = GetExample(propertyInfo);
                            if (!string.IsNullOrEmpty(example)) operation.RequestBody.Content["application/json"].Example = new OpenApiString(example);
                            break;
                        // default:
                        //     throw new ArgumentOutOfRangeException();
                    }
                }

                AddParameters(routeParameters, operation.Parameters);
                AddParameters(queryParameters, operation.Parameters);
                AddParameters(headerParameters, operation.Parameters);


                var formData = CreateSchemaAsForm(type);
                var formFiles = CreateSchemaAsFileForm(type);
                if (formFiles.Count > 0) {
                    formData ??= new OpenApiSchema {Type = "object"};
                    foreach (var formFile in formFiles) {
                        formData.Properties.Add(formFile.Key, formFile.Value);
                    }
                }

                if (formData is not null) {
                    var encoding = new Dictionary<string, OpenApiEncoding>();
                    foreach (var formDataItem in formData.Properties) {
                        encoding.Add(formDataItem.Key, new OpenApiEncoding {Style = ParameterStyle.Form});
                    }

                    var mediaType = new OpenApiMediaType {Schema = formData, Encoding = encoding};
                    operation.RequestBody = new OpenApiRequestBody {
                        Required = hasBody,
                        Content = {
                            ["application/x-www-form-urlencoded"] = mediaType, // new OpenApiMediaType {Schema = formData},
                            ["multipart/form-data"] = mediaType,
                        },
                    };
                    // continue;
                } else {
                    foreach (var propertyInfo in props) {
                        var propType = propertyInfo.PropertyType;
                        var fromBody = propertyInfo.GetCustomAttribute<FromBodyAttribute>();
                        if (fromBody is null || !hasBody || !propType.IsClass || !IsNotHttpContext(propType)) continue;

                        if (!_schema.TryGetValue(propType, out var schema)) {
                            schema = CreateSchema(propType);
                            _schema.Add(propType, schema);
                        }

                        operation.RequestBody = new OpenApiRequestBody {
                            Content = {{"application/json", new OpenApiMediaType {Schema = schema}}},
                            Description = GetSummary(propertyInfo),
                        };
                        var example = GetExample(propertyInfo);
                        if (!string.IsNullOrEmpty(example)) operation.RequestBody.Content["application/json"].Example = new OpenApiString(example);
                    }
                }

                paths[route].Operations.Add(operationType, operation);
            }
        }

        return paths;
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

    private static void AddParameters(IEnumerable<OpenApiParameter> source, ICollection<OpenApiParameter> destination) {
        foreach (var item in source) {
            destination.Add(item);
        }
    }

    private static bool IsRequired(PropertyInfo propertyInfo) {
        var propType = propertyInfo.PropertyType;
        return Nullable.GetUnderlyingType(propType) != null || propertyInfo.GetCustomAttribute<RequiredAttribute>() != null;
    }

    private static string ToCamelCase(ReadOnlySpan<char> value) {
        var length = value.Length;
        Span<char> result = stackalloc char[length];
        value.CopyTo(result);
        result[0] = char.ToLower(value[0]);
        return result.ToString();
    }

    private OpenApiSchema CreateSchema(Type type) {
        var properties = type.GetProperties();
        var schema = new OpenApiSchema();
        foreach (var propertyInfo in properties) {
            var jsonIgnore = propertyInfo.GetCustomAttribute<JsonIgnoreAttribute>();
            if (jsonIgnore is not null && jsonIgnore.Condition == JsonIgnoreCondition.Always) continue;
            var jsonPropertyName = propertyInfo.GetCustomAttribute<JsonPropertyNameAttribute>();
            var propertyName = jsonPropertyName?.Name ?? ToCamelCase(propertyInfo.Name);
            var propType = propertyInfo.PropertyType;
            var example = GetExample(propertyInfo);
            var propSchema = new OpenApiSchema {
                Type = GetOpenApiType(propType),
                Format = GetOpenApiFormat(propType),
                Pattern = GetOpenApiPattern(propType),
                Nullable = Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null,
                Example = example is null ? null : new OpenApiString(example)
            };
            schema.Properties.Add(propertyName, propSchema);
        }

        return schema;
    }

    private OpenApiSchema? CreateSchemaAsForm(Type type) {
        var properties = type.GetProperties();
        var schema = new OpenApiSchema {Type = "object"};
        var hasForm = false;
        foreach (var propertyInfo in properties) {
            var fromForm = propertyInfo.GetCustomAttribute<FromFormAttribute>();
            if (fromForm is null) continue;
            hasForm = true;
            var propertyName = fromForm.Name ?? propertyInfo.Name;
            var propType = propertyInfo.PropertyType;
            var example = GetExample(propertyInfo);
            var required = IsRequired(propertyInfo);
            var propSchema = new OpenApiSchema {
                Type = GetOpenApiType(propType),
                Format = GetOpenApiFormat(propType),
                Pattern = GetOpenApiPattern(propType),
                Nullable = !required, // Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null,
                Example = example is null ? null : new OpenApiString(example),
            };
            schema.Properties.Add(propertyName, propSchema);
        }

        return hasForm ? schema : null;
    }

    private IDictionary<string, OpenApiSchema> CreateSchemaAsFileForm(Type type) {
        var properties = type.GetProperties();
        // var schema = new OpenApiSchema {Type = "object"};
        var schemaList = new Dictionary<string, OpenApiSchema>();
        // var hasForm = false;
        foreach (var propertyInfo in properties) {
            var propType = propertyInfo.PropertyType;
            if (propType != typeof(IFormFile) || propType != typeof(IFormFileCollection)) {
                continue;
            }

            var bind = propertyInfo.GetCustomAttribute<FromBodyAttribute>();
            if (bind is null) continue;
            // hasForm = true;
            var propertyName = bind.Name ?? propertyInfo.Name;
            var example = GetExample(propertyInfo);
            var required = IsRequired(propertyInfo);
            var propSchema = new OpenApiSchema {
                Type = GetOpenApiType(propType),
                Format = "binary",
                Nullable = !required, // Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null,
                Example = example is null ? null : new OpenApiString(example),
            };
            schemaList.Add(propertyName, propSchema);
        }

        return schemaList; // hasForm ? schema : null;
    }

    private static string GetOpenApiType(Type type) {
        if (type == typeof(int) || type == typeof(short) || type == typeof(long) ||
            type == typeof(uint) || type == typeof(ushort) || type == typeof(ulong) ||
            type == typeof(sbyte) || type == typeof(byte)) return "integer";
        if (type == typeof(float) || type == typeof(double) || type == typeof(decimal)) return "number";
        if (type.GetCustomAttribute<EmailAddressAttribute>() != null) return "email";
        // if (type == typeof(TimeOnly) || type == typeof(TimeSpan)) return "pattern";
        // if (type == typeof(Guid)) return "string";
        // if (type == typeof(DateTime) || type == typeof(DateTimeOffset)) return "string";
        // if (type == typeof(DateOnly)) return "string";
        // if (type == typeof(TimeOnly)) return "string";
        if (type == typeof(bool)) return "boolean";
        // if (type == typeof(IFormFile) || type.GetInterfaces().Contains(typeof(IFormFile))) return "string";
        // if (type == typeof(b)) return "boolean";

        return "string"; // type.Name.ToLower();
    }

    private static string? GetOpenApiPattern(Type type) {
        if (type == typeof(TimeOnly) || type == typeof(TimeSpan)) return "00:00:00";
        if (type == typeof(byte[])) return Convert.ToBase64String(new byte[] {00, 00, 00, 00, 00});
        return null;
    }

    private static string GetOpenApiFormat(Type type) {
        if (type == typeof(int)) return "int32";
        if (type == typeof(uint)) return "uint32";
        if (type == typeof(short)) return "int16";
        if (type == typeof(ushort)) return "uint16";
        if (type == typeof(long)) return "int64";
        if (type == typeof(ulong)) return "uint64";
        if (type == typeof(float)) return "float";
        if (type == typeof(double)) return "double";
        if (type == typeof(decimal)) return "decimal";
        if (type == typeof(bool)) return "boolean";
        if (type == typeof(DateTime)) return "date-time";
        if (type == typeof(DateOnly)) return "date";
        if (type == typeof(TimeOnly)) return "^asd$";
        if (type == typeof(DateTimeOffset)) return "date-time";
        if (type == typeof(TimeSpan)) return "duration";
        if (type == typeof(Guid)) return "uuid";
        if (type == typeof(string)) return "string";
        if (type == typeof(byte) || type == typeof(sbyte)) return "int32";
        if (type == typeof(byte[])) return "byte";
        if (type == typeof(IFormFile) || type.GetInterfaces().Contains(typeof(IFormFile))) return "binary";
        return "string";
    }

    private static bool IsNotHttpContext(Type type)
        => type != typeof(HttpContext) && type != typeof(HttpRequest) && type != typeof(HttpResponse);

    private static bool IsHttpContext(Type type)
        => type == typeof(HttpContext) || type == typeof(HttpRequest) || type == typeof(HttpResponse);
}
