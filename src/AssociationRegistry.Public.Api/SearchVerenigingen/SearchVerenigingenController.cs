namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caches;
using Examples;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class SearchVerenigingenController : ApiController
{
    /// <summary>
    /// Zoek verenigingen op. (statische dataset)
    /// </summary>
    /// <response code="200">Indien de zoekopdracht succesvol was.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("zoeken")]
    [ProducesResponseType(typeof(SearchVerenigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(contentType: WellknownMediaTypes.Json)]
    public async Task<IActionResult> Get([FromServices] IVerenigingenRepository verenigingenRepository) =>
        await Task.FromResult<IActionResult>(Ok(verenigingenRepository.Verenigingen));

    [HttpPut("zoeken")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Put(
        [FromServices] IVerenigingenRepository verenigingenRepository,
        [FromBody] ImmutableArray<Vereniging>? maybeBody,
        CancellationToken cancellationToken)
    {
        if (maybeBody is not { }body)
            return BadRequest();

        await verenigingenRepository.UpdateVerenigingen(body, Request.Body, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Zoek verenigingen op. (statische dataset)
    /// </summary>
    /// <response code="200">Indien de zoekopdracht succesvol was.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("zoeken2")]
    [ProducesResponseType(typeof(SearchVerenigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(contentType: WellknownMediaTypes.Json)]
    public async Task<IActionResult> Zoeken([FromServices] ElasticClient elasticClient, [FromQuery] string q)
    {
        var searchResponse = await Search(q, elasticClient);

        return Ok(searchResponse.Hits.Select(x => x.Source));
    }

    public static async Task<ISearchResponse<VerenigingDocument>> Search(string q, ElasticClient client)
    {
        return await client.SearchAsync<VerenigingDocument>(
            s => s
                .From(0)
                .Size(10)
                .Query(
                    query => query
                        .Bool(
                            b => b
                                .Must(m => m.QueryString(qs => qs.Query(q)))))
        );
    }
}
