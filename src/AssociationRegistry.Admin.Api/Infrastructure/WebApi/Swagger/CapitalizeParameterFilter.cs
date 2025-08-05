namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class CapitalizeParameterFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (operation.Parameters == null) return;

        foreach (var parameter in operation.Parameters)
        {
            if (parameter.In != ParameterLocation.Header) continue;

            parameter.Name = char.ToUpper(parameter.Name[0]) + parameter.Name.Substring(1);
        }
    }
}
