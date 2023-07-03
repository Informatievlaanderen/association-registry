namespace AssociationRegistry.Admin.Api.Infrastructure.Swagger;

using System.Collections.Generic;
using Middleware;
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
                Name = CorrelationIdMiddleware.CorrelationIdHeader,
                In = ParameterLocation.Header,
                Description = "Deze id identificeert de request",
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "guid",
                    Format = "00000000-0000-0000-0000-00000000",
                    Nullable = false,
                },
            });
    }
}
