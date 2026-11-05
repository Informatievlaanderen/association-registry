namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger;

using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

public class AppendCorrelationIdToHeaders : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<IOpenApiParameter>();

        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = WellknownHeaderNames.CorrelationId,
                In = ParameterLocation.Header,
                Description = "Deze id identificeert de request.",
                Required = true,
                Schema = new OpenApiSchema { Type = JsonSchemaType.String, Format = "uuid" },
            }
        );
    }
}
