namespace AssociationRegistry.Admin.Api.Administratie.DubbelControle;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using DecentraalBeheer.Verenigingen.Registreer.FeitelijkeVereniging.RequetsModels;
using DuplicateVerenigingDetection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("projections")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class DubbelControleController : ApiController
{
    [HttpPost("admin/dubbelcontrole")]
    public async Task<IActionResult> ControleerOpDubbels(
        [FromBody] RegistreerFeitelijkeVerenigingRequest? request,
        [FromQuery] double? minimumScoreOverride,
        [FromServices] IDuplicateVerenigingDetectionService duplicateVerenigingDetectionService
    )
    {
        var command = request.ToCommand();

        var result = await duplicateVerenigingDetectionService.GetDuplicates(
            command.Naam, command.Locaties,
            true,
            minimumScoreOverride.HasValue
                ? new MinimumScore(minimumScoreOverride.Value)
                : MinimumScore.Default);

        return Ok(result);
    }
}
