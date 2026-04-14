namespace AssociationRegistry.Admin.Api.WebApi.Administratie.ScheduledHost;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Infrastructure.HttpClients;
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
public class ScheduledHostController : ApiController
{
    [HttpPost("scheduledhost/trigger/bewaartermijn")]
    public async Task<IActionResult> Get(
    [FromServices] ScheduledHostHttpClient client,
        [FromServices] ILogger<ScheduledHostController> logger,
        CancellationToken cancellationToken
    )
    {
        await client.TriggerBewaartermijnScheduledHost(cancellationToken);

        return Ok();
    }
}
