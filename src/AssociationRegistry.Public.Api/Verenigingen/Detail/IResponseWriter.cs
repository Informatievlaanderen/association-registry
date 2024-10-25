namespace AssociationRegistry.Public.Api.Verenigingen.Detail;

using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.AspNetCore.Mvc;

public interface IResponseWriter
{
    IActionResult WriteResponse(ApiController controller, string presignedUrl);
}

public class ResponseWriter : IResponseWriter
{
    public IActionResult WriteResponse(ApiController controller, string presignedUrl)
    {
        controller.Response.Headers.Location = presignedUrl;

        return controller.StatusCode(StatusCodes.Status307TemporaryRedirect);
    }
}
