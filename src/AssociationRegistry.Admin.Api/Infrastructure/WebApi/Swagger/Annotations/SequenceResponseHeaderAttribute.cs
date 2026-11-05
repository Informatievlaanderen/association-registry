namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;

using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.Filters;

public class SequenceResponseHeaderAttribute : SwaggerResponseHeaderAttribute
{
    public SequenceResponseHeaderAttribute(int statusCode = Microsoft.AspNetCore.Http.StatusCodes.Status202Accepted)
        : base(
            statusCode,
            WellknownHeaderNames.Sequence,
            type: JsonSchemaType.String,
            description: "Het sequence nummer van deze request."
        ) { }
}
