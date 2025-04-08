namespace AssociationRegistry.Admin.Api.Administratie.NutsLauInfo;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Grar.NutsLau;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

/// <summary>
/// SuperAdmin endpoint to manually call.
/// This sync the nuts and lau info from grar.
/// </summary>
[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("admin")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class SyncNutsLauInfoController : ApiController
{
    [HttpPost("syncnutslauinfo")]
    public async Task<IActionResult> QueueAdressenForAdresMatch(
        [FromServices] INutsAndLauSyncService nutsAndLauSyncService,
        [FromServices] ILogger<SyncNutsLauInfoController> logger,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Start SyncNutsLauInfoController");

        await nutsAndLauSyncService.SyncNutsLauInfo(cancellationToken);

        return Ok();
    }
}
