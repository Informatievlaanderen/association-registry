namespace AssociationRegistry.Admin.ProjectionHost.Infrastructure;

using System.Reflection;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

[ApiVersionNeutral]
[Route("")]
public class EmptyController : Controller
{
    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Get()
        => Request.Headers[HeaderNames.Accept].ToString().Contains("text/html")
            ? new RedirectResult("/docs")
            : new OkObjectResult($"Welcome to the Basisregisters Vlaanderen Publieke Api {Assembly.GetEntryAssembly()!.GetVersionText()}.");
}
