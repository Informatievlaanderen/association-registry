namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;

using Swashbuckle.AspNetCore.Filters;

public class SequenceResponseHeaderAttribute : SwaggerResponseHeaderAttribute
{
    public SequenceResponseHeaderAttribute(int statusCode = Microsoft.AspNetCore.Http.StatusCodes.Status202Accepted)
        : base(
            statusCode,
            WellknownHeaderNames.Sequence,
            type: "string",
            description: "Het sequence nummer van deze request."
        ) { }
}
