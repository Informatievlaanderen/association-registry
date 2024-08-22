namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using EventStore;
using Extensions;

public class UnexpectedAggregateVersionMiddleware
{
    private readonly RequestDelegate _next;

    public UnexpectedAggregateVersionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ProblemDetailsHelper problemDetailsHelper)
    {
        try
        {
            await _next(context);
        }
        catch (UnexpectedAggregateVersionException ex)
        {
            await context.Response.WriteProblemDetailsAsync(problemDetailsHelper, ex.Message, StatusCodes.Status412PreconditionFailed);
        }
    }
}
