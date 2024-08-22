namespace AssociationRegistry.Admin.Api.Infrastructure.Swagger;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class AppendCorrelationIdToHeaders : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = WellknownHeaderNames.CorrelationId,
                In = ParameterLocation.Header,
                Description = "Deze id identificeert de request.",
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Format = "uuid",
                    Nullable = false,
                },
            });
    }
}
