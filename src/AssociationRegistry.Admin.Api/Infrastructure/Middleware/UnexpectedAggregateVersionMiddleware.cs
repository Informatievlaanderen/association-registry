namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

using System.Threading.Tasks;
using EventStore;
using Microsoft.AspNetCore.Http;

public class UnexpectedAggregateVersionMiddleware
{
    private readonly RequestDelegate _next;

    public UnexpectedAggregateVersionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (UnexpectedAggregateVersionException)
        {
            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status412PreconditionFailed;
        }
    }
}
