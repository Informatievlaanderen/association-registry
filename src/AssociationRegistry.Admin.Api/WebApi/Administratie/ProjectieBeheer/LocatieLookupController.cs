namespace AssociationRegistry.Admin.Api.WebApi.Administratie.ProjectieBeheer;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using Schema.Locaties;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("projections")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class LocatieLookupController : ApiController
{
    [HttpGet("admin/locaties/gekoppeldmetgrar/vCode/{vCode}")]
    public async Task<IActionResult> GetLocatiesMetAdresIdVolgensVCode(
        [FromServices] IDocumentStore documentStore,
        [FromRoute] string vCode,
        CancellationToken cancellationToken
    )
    {
        await using var session = documentStore.LightweightSession();

        var locaties = await session
                .Query<LocatieLookupDocument>()
                .Where(w => w.VCode == vCode)
                .Select(s => new LocatiesMetAdresIdVolgensVCode.LocatieLookup(s.LocatieId, s.AdresId))
            .ToListAsync(token: cancellationToken);

        var response = new LocatiesMetAdresIdVolgensVCode { VCode = vCode, Data = locaties.ToArray() };

        return Ok(response);
    }

    [HttpGet("admin/locaties/gekoppeldmetgrar/adresId/{adresId}")]
    public async Task<IActionResult> GetLocatiesMetAdresIdVolgensAdresId(
        [FromServices] IDocumentStore documentStore,
        [FromRoute] string adresId,
        CancellationToken cancellationToken
    )
    {
        await using var session = documentStore.LightweightSession();

        var locaties = await session
                .Query<LocatieLookupDocument>()
                .Where(w => w.AdresId == adresId)
                .Select(s => new LocatiesMetAdresIdVolgensAdresId.LocatieLookup(s.LocatieId, s.VCode))
            .ToListAsync(token: cancellationToken);

        var response = new LocatiesMetAdresIdVolgensAdresId { AdresId = adresId, Data = locaties.ToArray() };

        return Ok(response);
    }
}
