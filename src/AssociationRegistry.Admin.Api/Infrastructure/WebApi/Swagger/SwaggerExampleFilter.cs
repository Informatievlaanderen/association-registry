namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger;

using System.Reflection;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = true)]
public class SwaggerParameterExampleAttribute : SwaggerParameterExampleBaseAttribute
{
    public SwaggerParameterExampleAttribute(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public SwaggerParameterExampleAttribute(string value)
    {
        Name = value;
        Value = value;
    }

    public string Name { get; }
    public string Value { get; }
}

public abstract class SwaggerParameterExampleBaseAttribute : Attribute { }

public class ExampleSchemaFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsClass)
            return;

        foreach (var pi in context.Type.GetProperties())
        {
            var attribs = pi.GetCustomAttributes<SwaggerParameterExampleBaseAttribute>().ToArray();

            if (attribs.Length <= 0)
                continue;

            var prop = schema.Properties?.FirstOrDefault(x =>
                x.Key.Equals(pi.Name, StringComparison.OrdinalIgnoreCase)
            );

            if (prop?.Value is not OpenApiSchema propertySchema)
                continue;

            var list = new List<string>();

            list.AddRange(attribs.OfType<SwaggerParameterExampleAttribute>().Select(x => x.Value));
            propertySchema.Example = JsonValue.Create(list.First());

            if (list.Count > 1)
                propertySchema.Description +=
                    $"\r\n\r\n" + $"Mogelijke waarden:\r\n" + $"- {string.Join(separator: "\r\n- ", list)}";
        }
    }
}
