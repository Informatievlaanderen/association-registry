namespace AssociationRegistry.Admin.Api.WebApi.Administratie.DubbelControle;

using Asp.Versioning;
using AssociationRegistry.DuplicateVerenigingDetection;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verenigingen.Registreer.FeitelijkeVereniging.RequestModels;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("admin")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class DubbelControleController : ApiController
{
    [HttpPost("dubbelcontrole")]
    public async Task<IActionResult> ControleerOpDubbels(
        [FromBody] RegistreerFeitelijkeVerenigingRequest? request,
        [FromQuery] double? minimumScoreOverride,
        [FromServices] MinimumScore defaultMinimumScore,
        [FromServices] IDuplicateVerenigingDetectionService duplicateVerenigingDetectionService,
        [FromServices] IWerkingsgebiedenService werkingsgebiedenService
    )
    {
        var command = request.ToCommand(werkingsgebiedenService);

        var result = await duplicateVerenigingDetectionService.ExecuteAsync(
            command.Naam, command.Locaties,
            true,
            minimumScoreOverride.HasValue
                ? new MinimumScore(minimumScoreOverride.Value)
                : defaultMinimumScore);

        return Ok(result.Select(x => new DubbelControleResponse(x.VCode,
                                                                x.Naam,
                                                                x.Scoring.Explanation,
                                                                x.Scoring.Score.Value)));
    }
}
public class DubbelControleResponse
{
    public DubbelControleResponse()
    {

    }

    public DubbelControleResponse(string vCode, string naam, string explanation, double score)
    {
        VCode = vCode;
        Naam = naam;
        Explanation = explanation;
        Score = score;
    }

    public string VCode { get; set; }
    public string Naam { get; }
    public string Explanation { get; set; }
    public double Score { get; set; }
}
