namespace AssociationRegistry.Admin.Api.Contexten;

using Be.Vlaanderen.Basisregisters.Api;
using Constants;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("contexten")]
[ApiExplorerSettings(GroupName = "Contexten")]
public class ContextenController : ApiController
{
    /// <summary>
    ///     Vraag de JSON-LD context op voor een specifiek object
    /// </summary>
    /// <param name="name">Dit is de naam van de specifieke context</param>
    /// <returns></returns>
    [HttpGet("{name}")]
    public IActionResult Detail([FromRoute] string name)
    {
        try
        {
            var context = JsonLdContexts.GetContext(name);

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
