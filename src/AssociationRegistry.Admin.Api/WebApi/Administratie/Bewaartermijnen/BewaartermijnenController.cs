namespace AssociationRegistry.Admin.Api.WebApi.Administratie.Bewaartermijnen;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// SuperAdmin endpoint to manually call.
/// This will try to match any location without an AdresId.
/// </summary>
[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("admin")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class BewaartermijnenController : ApiController
{
    [HttpGet("bewaartermijnen/{vCode}/{vertegenwoordigerId}")]
    public async Task<IActionResult> Get(
        [FromRoute] string vCode,
        [FromRoute] string vertegenwoordigerId,
       // [FromServices] IBewaartermijnenQuery documentStore,
        [FromServices] ILogger<BewaartermijnenController> logger,
        CancellationToken cancellationToken)
    {
        return NotFound();
    }
}

public record BewaartermijnResponse();
