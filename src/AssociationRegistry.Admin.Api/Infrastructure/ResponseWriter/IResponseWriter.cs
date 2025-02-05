namespace AssociationRegistry.Admin.Api.Infrastructure.ResponseWriter;

using Microsoft.AspNetCore.Mvc;

public interface IResponseWriter
{
    Task<IActionResult> WriteNotFoundProblemDetailsAsync(
        HttpResponse response,
        string? message = null);

    void AddETagHeader(HttpResponse response, long? version);
    void AddSequenceHeader(HttpResponse response, long? sequence);
}
