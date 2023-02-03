namespace AssociationRegistry.Admin.Api.Infrastructure;

using System.Reflection;
using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

[ApiVersionNeutral]
[Route("")]
public class EmptyController : ApiController
{
    [HttpGet]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public IActionResult Get()
        => Request.Headers[HeaderNames.Accept].ToString().Contains("text/html")
            ? new RedirectResult("/docs")
            : new OkObjectResult($"Welcome to the Basisregisters Vlaanderen Beheer Api {Assembly.GetEntryAssembly()!.GetVersionText()}.");
}
