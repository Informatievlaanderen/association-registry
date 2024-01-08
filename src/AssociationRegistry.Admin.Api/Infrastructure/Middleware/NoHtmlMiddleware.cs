namespace AssociationRegistry.Admin.Api.Infrastructure.Middleware;

using Microsoft.AspNetCore.Http;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class NoHtmlMiddleware
{
    private readonly RequestDelegate _next;

    public NoHtmlMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.EnableBuffering();

        var requestContent = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0; // Reset the stream position for further processing

        if (ContainsHtml(requestContent))
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync("HTML content is not allowed.");
            return;
        }

        await _next(context);
    }

    private bool ContainsHtml(string content)
    {
        // Simple HTML detection logic (can be improved or made more complex as needed)
        return Regex.IsMatch(content, "<.*?>");
    }
}