namespace AssociationRegistry.Admin.Api.Infrastructure.ResponseWriter;

using Extensions;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;

public class ResponseWriter : IResponseWriter
{
    private readonly ProblemDetailsHelper _problemDetailsHelper;

    public ResponseWriter(ProblemDetailsHelper problemDetailsHelper)
    {
        _problemDetailsHelper = problemDetailsHelper;
    }

    public async Task<IActionResult> WriteNotFoundProblemDetailsAsync(HttpResponse response, string? message = null)
        => await response.WriteNotFoundProblemDetailsAsync(_problemDetailsHelper, message);

    public void AddETagHeader(HttpResponse response, long? version)
        => response.AddETagHeader(version);

    public void AddSequenceHeader(HttpResponse response, long? sequence)
        => response.AddSequenceHeader(sequence);
}
