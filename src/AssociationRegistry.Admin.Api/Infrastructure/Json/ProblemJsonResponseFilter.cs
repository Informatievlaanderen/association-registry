namespace AssociationRegistry.Admin.Api.Infrastructure.Json;

using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class ProblemJsonResponseFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var (_, value) in operation.Responses.Where(
                     entry =>
                         (entry.Key.StartsWith("4") && entry.Key != "400") ||
                         entry.Key.StartsWith("5")))
        {
            if (!value.Content.Any())
                return;

            var openApiMediaType = value.Content.First().Value;

            value.Content.Clear();

            value.Content.Add(
                new KeyValuePair<string, OpenApiMediaType>(key: "application/problem+json", openApiMediaType));
        }
    }
}
