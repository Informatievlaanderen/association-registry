namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;

using Swashbuckle.AspNetCore.Filters;

public class ETagResponseHeaderAttribute : SwaggerResponseHeaderAttribute
{
    public ETagResponseHeaderAttribute(
        string description,
        int statusCode = Microsoft.AspNetCore.Http.StatusCodes.Status202Accepted
    )
        : base(statusCode, name: "ETag", type: "string", description: description) { }
}
