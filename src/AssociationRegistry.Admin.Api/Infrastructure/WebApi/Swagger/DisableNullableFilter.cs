namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class DisableNullableFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        foreach (var prop in schema.Properties)
        {
            prop.Value.Nullable = false;
        }
    }
}
