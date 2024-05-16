namespace AssociationRegistry.Admin.Api.ProjectieBeheer;

using Be.Vlaanderen.Basisregisters.Api;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using Schema.Detail;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

[Authorize(Policy = Program.SuperAdminPolicyName)]
public partial class ProjectionController : ApiController
{
    [HttpGet("admin/locaties/lookup/vCode/{vCode}")]
    public async Task<IActionResult> GetLocatiesMetAdresIdVolgensVCode(
        [FromServices] IDocumentStore documentStore,
        [FromRoute] string vCode,
        CancellationToken cancellationToken)
    {
        await using var session = documentStore.LightweightSession();

        var doc = await session.LoadAsync<LocatieLookupDocument>(vCode);

        var response = new LocatiesMetAdresIdVolgensVCode
        {
            VCode = vCode,
            Data = doc.Locaties.Select(loc => new LocatiesMetAdresIdVolgensVCode.LocatieLookup(loc.LocatieId, loc.AdresId)),
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
            Data = Array.Empty<LocatiesMetAdresIdVolgensAdresId.LocatieLookup>(),
        };

        var docs = session.Query<LocatieLookupDocument>()
                          .Where(w => w.Locaties.Any(loc => loc.AdresId == adresId))
                          .ToArray();

        foreach (var doc in docs)
        {
            var locaties = doc.Locaties
                .Where(loc => loc.AdresId == adresId)
                .Select(loc => new LocatiesMetAdresIdVolgensAdresId.LocatieLookup(loc.LocatieId, doc.VCode));
            foreach(var locatie in locaties) response.Data = response.Data.Append(locatie);
        }

        return Ok(response);
    }
}
