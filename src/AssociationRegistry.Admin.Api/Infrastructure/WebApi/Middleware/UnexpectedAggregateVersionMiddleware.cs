namespace AssociationRegistry.Admin.Api.Infrastructure.WebApi.Middleware;

using AssociationRegistry.EventStore;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;

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
