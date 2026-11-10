namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;

using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;

public class ETagResponseHeaderAttribute : SwaggerResponseHeaderAttribute
{
    public ETagResponseHeaderAttribute(
        string description,
        int statusCode = Microsoft.AspNetCore.Http.StatusCodes.Status202Accepted
    )
        : base(statusCode, name: "ETag", type: JsonSchemaType.String, description: description) { }
}
