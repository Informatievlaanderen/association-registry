namespace AssociationRegistry.Admin.Api.ProjectieBeheer;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using Schema.Detail;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("projections")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class LocatieLookupController : ApiController
{
    [HttpGet("admin/locaties/lookup/vCode/{vCode}")]
    public async Task<IActionResult> GetLocatiesMetAdresIdVolgensVCode(
        [FromServices] IDocumentStore documentStore,
        [FromRoute] string vCode,
        CancellationToken cancellationToken)
    {
        await using var session = documentStore.LightweightSession();

        var response = new LocatiesMetAdresIdVolgensVCode
        {
            VCode = vCode,
            Data = session.Query<LocatieLookupDocument>()
                          .Where(w => w.VCode == vCode)
                          .Select(s => new LocatiesMetAdresIdVolgensVCode.LocatieLookup(s.LocatieId, s.AdresId))
                          .ToArray(),
        };

        return Ok(response);
    }

    [HttpGet("admin/locaties/lookup/adresId/{adresId}")]
    public async Task<IActionResult> GetLocatiesMetAdresIdVolgensAdresId(
        [FromServices] IDocumentStore documentStore,
        [FromRoute] string adresId,
        CancellationToken cancellationToken)
    {
        await using var session = documentStore.LightweightSession();

        var response = new LocatiesMetAdresIdVolgensAdresId
        {
            AdresId = adresId,
            Data = session.Query<LocatieLookupDocument>()
                          .Where(w => w.AdresId == adresId)
                          .Select(s => new LocatiesMetAdresIdVolgensAdresId.LocatieLookup(s.LocatieId, s.VCode))
                          .ToArray(),
        };

        return Ok(response);
    }
}
