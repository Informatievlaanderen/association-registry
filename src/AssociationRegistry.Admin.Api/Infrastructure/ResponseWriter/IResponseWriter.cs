namespace AssociationRegistry.Admin.Api.Infrastructure.ResponseWriter;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;

public interface IResponseWriter
{
    Task<IActionResult> WriteNotFoundProblemDetailsAsync(
        HttpResponse response,
        string? message = null);

    void AddETagHeader(HttpResponse response, long? version);
}
