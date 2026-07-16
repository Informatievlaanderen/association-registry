namespace AssociationRegistry.Admin.Api.WebApi.Maintenance;

using Administratie.Configuratie;
using Asp.Versioning;
using AssociationRegistry.EventStore;
using Be.Vlaanderen.Basisregisters.Api;
using DecentraalBeheer.Vereniging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("maintenance")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class StreamArchivalController : ApiController
{
    [HttpDelete("streams/{vCode}")]
    public async Task<IActionResult> ArchiveStream(
        [FromServices] IStreamArchivalService streamArchivalService,
        string vCode
    )
    {
        await streamArchivalService.ArchiveStream(VCode.Create(vCode));

        return NoContent();
    }
}
