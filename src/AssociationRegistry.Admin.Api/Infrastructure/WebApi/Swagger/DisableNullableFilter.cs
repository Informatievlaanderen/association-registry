namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger;

using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

public class DisableNullableFilter : ISchemaFilter
{
    public void Apply(IOpenApiSchema schema, SchemaFilterContext context)
    {
        foreach (var prop in schema.Properties)
        {
            if (prop.Value is not OpenApiSchema { Type: not null } propertySchema)
                continue;

            propertySchema.Type &= ~JsonSchemaType.Null;
        }
    }
}
