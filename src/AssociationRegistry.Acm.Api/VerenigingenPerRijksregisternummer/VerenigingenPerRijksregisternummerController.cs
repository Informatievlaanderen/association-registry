using System.IO;
using System.Threading;
using AssociationRegistry.Acm.Api.Caches;
using AssociationRegistry.Acm.Api.Infrastructure;
using AssociationRegistry.Acm.Api.S3;
using Be.Vlaanderen.Basisregisters.BlobStore;

namespace AssociationRegistry.Acm.Api.VerenigingenPerRijksregisternummer;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class VerenigingenPerRijksregisternummerController : ApiController
{
    private BlobName _blobName;

    /// <summary>
    /// Vraag de lijst van verenigingen voor een rijksregisternummer op.
    /// </summary>
    /// <param name="rijksregisternummer"></param>
    /// <response code="200">Als het rijksregisternummer gevonden is.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet]
    [ProducesResponseType(typeof(GetVerenigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    public async Task<IActionResult> Get(
        [FromServices] Data data,
        [FromQuery] string rijksregisternummer)
    {
        var maybeKey = CalculateKey(rijksregisternummer);
        if (maybeKey is not { } key || !data.Verenigingen.ContainsKey(key))
            return Ok(new GetVerenigingenResponse(rijksregisternummer, ImmutableArray<Vereniging>.Empty));

        return Ok(new GetVerenigingenResponse(rijksregisternummer, data.Verenigingen[key]));
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromServices] VerenigingenBlobClient blobClient, [FromServices] Data data, [FromBody] Dictionary<string, Dictionary<string, string>>? maybeBody, CancellationToken cancellationToken)
    {
        if (maybeBody is not { } || !Data.TryParse(maybeBody, out var verenigingen)) return BadRequest();
        
        _blobName = new BlobName("data.json");
        await blobClient.DeleteBlobAsync(_blobName, cancellationToken);
        await blobClient.CreateBlobAsync(_blobName, Metadata.None, ContentType.Parse("application/json"), Request.Body, cancellationToken);

        data.Verenigingen = verenigingen;
        
        return Ok();
    }
    
    // [ApiExplorerSettings(IgnoreApi = true)]
    // public async Task<IActionResult> Put([FromQuery] string rijksregisternummer, [FromBody] PutVerenigingenRequest request)
    // {
    //     var maybeKey = CalculateKey(rijksregisternummer);
    //     if (maybeKey is not { } key)
    //         return BadRequest();
    //
    //     var newVerenigingen = ImmutableArray<Vereniging>.Empty;
    //
    //     newVerenigingen = request.Verenigingen
    //         .Aggregate(newVerenigingen, (current, vereniging) => current.Add(new Vereniging(vereniging.Id, vereniging.Naam)));
    //
    //     if (Verenigingen.ContainsKey(key))
    //         Verenigingen[key] = newVerenigingen;
    //     else
    //         Verenigingen.Add(key, newVerenigingen);
    //
    //     return await Task.FromResult(Ok());
    // }

    private static string? CalculateKey(string rijksregisternummer)
        => rijksregisternummer.Length < 4 ? null : rijksregisternummer[..4];
}
