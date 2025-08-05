namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger;

using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

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

public abstract class SwaggerParameterExampleBaseAttribute : Attribute
{
}

public class ExampleSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (!context.Type.IsClass) return;

        foreach (var pi in context.Type.GetProperties())
        {
            var attribs = pi.GetCustomAttributes<SwaggerParameterExampleBaseAttribute>().ToArray();

            if (attribs.Length <= 0) continue;

            var prop = schema.Properties.FirstOrDefault(x => x.Key.Equals(pi.Name, StringComparison.OrdinalIgnoreCase));

            if (prop.Equals(default(KeyValuePair<string, OpenApiSchema>))) continue;

            var list = new List<string>();

            list.AddRange(attribs.OfType<SwaggerParameterExampleAttribute>().Select(x => x.Value));
            prop.Value.Example = new OpenApiString(list.First());

            if (list.Count > 1)
                prop.Value.Description += $"\r\n\r\n" +
                                          $"Mogelijke waarden:\r\n" +
                                          $"- {string.Join(separator: "\r\n- ", list)}";
        }
    }
}
