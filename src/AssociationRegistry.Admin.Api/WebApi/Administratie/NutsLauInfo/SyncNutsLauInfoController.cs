namespace AssociationRegistry.Admin.Api.WebApi.Administratie.NutsLauInfo;

using Asp.Versioning;
using AssociationRegistry.Grar.NutsLau;
using Be.Vlaanderen.Basisregisters.Api;
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
    public async Task<IActionResult> Sync(
        [FromServices] INutsAndLauSyncService nutsAndLauSyncService,
        [FromServices] ILogger<SyncNutsLauInfoController> logger)
    {
        logger.LogInformation("Start SyncNutsLauInfoController");

        await nutsAndLauSyncService.SyncNutsLauInfo(CancellationToken.None);

        return Ok();
    }
}
