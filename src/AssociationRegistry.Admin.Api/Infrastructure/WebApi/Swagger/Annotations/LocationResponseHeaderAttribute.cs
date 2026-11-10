namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;

using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;

public class LocationResponseHeaderAttribute : SwaggerResponseHeaderAttribute
{
    public LocationResponseHeaderAttribute(
        string description,
        int statusCode = Microsoft.AspNetCore.Http.StatusCodes.Status202Accepted
    )
        : base(statusCode, name: "Location", type: JsonSchemaType.String, description: description) { }
}
