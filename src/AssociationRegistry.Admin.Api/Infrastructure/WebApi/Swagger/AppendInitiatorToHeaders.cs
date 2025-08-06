namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class AppendInitiatorToHeaders : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(
            new OpenApiParameter
            {
                Name = WellknownHeaderNames.Initiator,
                In = ParameterLocation.Header,
                Description = "Initiator header met als waarde de instantie die de wijziging uitvoert.",
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string",
                    Nullable = false,
                },
            });
    }
}
