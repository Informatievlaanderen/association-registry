namespace AssociationRegistry.Public.Api.Contexten;

using AssociationRegistry.Public.Api.Constants;
using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("contexten")]
[ApiExplorerSettings(GroupName = "Contexten")]
public class ContextenController : ApiController
{
    [HttpGet("{name}")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult Detail([FromRoute] string name)
        => Content(
            JsonLdContexts.GetContext(name),
            WellknownMediaTypes.Json);
}
