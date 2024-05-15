namespace AssociationRegistry.Admin.Api.ProjectieBeheer;

using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using EventStore;
using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.HttpClients;
using Marten;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using Schema.Detail;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
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
                          .Select(s => new LocatiesMetAdresIdVolgensAdresId.LocatieLookup(s.VCode, s.LocatieId))
                          .ToArray(),
        };

        return Ok(response);
    }

    [HttpGet("admin/locaties/lookup/locatieId/{locatieId}")]
    public async Task<IActionResult> GetLocatiesMetAdresIdVolgensLocatieId(
        [FromServices] IDocumentStore documentStore,
        [FromRoute] int locatieId,
        CancellationToken cancellationToken)
    {
        await using var session = documentStore.LightweightSession();

        var response = new LocatiesMetAdresIdVolgensLocatieId
        {
            LocatieId = locatieId,
            Data = session.Query<LocatieLookupDocument>()
                          .Where(w => w.LocatieId == locatieId)
                          .Select(s => new LocatiesMetAdresIdVolgensLocatieId.LocatieLookup(s.VCode, s.AdresId))
                          .ToArray(),
        };

        return Ok(response);
    }
}
