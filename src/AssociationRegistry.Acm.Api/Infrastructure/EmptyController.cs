namespace AssociationRegistry.Acm.Api.Infrastructure;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System.Reflection;

[ApiVersionNeutral]
[Route("")]
public class EmptyController : ApiController
{
    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Get()
        => Request.Headers[HeaderNames.Accept].ToString().Contains("text/html")
            ? new RedirectResult("/docs")
            : new OkObjectResult($"Welcome to the Basisregisters Vlaanderen Acm Api {Assembly.GetEntryAssembly()!.GetVersionText()}.");
}
