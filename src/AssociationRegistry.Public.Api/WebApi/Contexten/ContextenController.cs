namespace AssociationRegistry.Public.Api.WebApi.Contexten;

using Asp.Versioning;
using AssociationRegistry.Public.Api.Constants;
using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("contexten")]
[ApiExplorerSettings(GroupName = "Contexten")]
public class ContextenController : ApiController
{
    /// <summary>
    ///     Vraag de JSON-LD context op voor een specifiek object binnen de publieke api
    /// </summary>
    /// <param name="name">Dit is de naam van de specifieke context</param>
    /// <returns></returns>
    [HttpGet("publiek/{name}")]
    public IActionResult PubliekContext([FromRoute] string name)
    {
        try
        {
            var context = JsonLdContexts.GetContext(folder: "publiek", name);

            return Content(
                context,
                WellknownMediaTypes.Json);
        }
        catch
        {
            return NotFound();
        }
    }

    /// <summary>
    ///     Vraag de JSON-LD context op voor een specifiek object binnen de beheer api
    /// </summary>
    /// <param name="name">Dit is de naam van de specifieke context</param>
    /// <returns></returns>
    [HttpGet("beheer/{name}")]
    public IActionResult BeheerContext([FromRoute] string name)
    {
        try
        {
            var context = JsonLdContexts.GetContext(folder: "beheer", name);

            return Content(
                context,
                WellknownMediaTypes.Json);
        }
        catch
        {
            return NotFound();
        }
    }
}
