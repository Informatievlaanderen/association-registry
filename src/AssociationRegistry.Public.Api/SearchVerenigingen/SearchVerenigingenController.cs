namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Caches;
using Constants;
using Examples;
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
    /// Zoek verenigingen op. (statische dataset, momenteel niet gebruikt)
    /// </summary>
    /// <response code="200">Indien de zoekopdracht succesvol was.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    // [HttpGet("zoeken")]
    // [ProducesResponseType(typeof(SearchVerenigingenResponse), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    // [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchVerenigingenResponseExamples))]
    // [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    // [Produces(contentType: WellknownMediaTypes.Json)]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Get([FromServices] IVerenigingenRepository verenigingenRepository)
        => await Task.FromResult<IActionResult>(Ok(verenigingenRepository.Verenigingen));

    [HttpPut("zoeken")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public async Task<IActionResult> Put(
        [FromServices] IVerenigingenRepository verenigingenRepository,
        [FromBody] ImmutableArray<Vereniging>? maybeBody,
        CancellationToken cancellationToken)
    {
        if (maybeBody is not { } body)
            return BadRequest();

        await verenigingenRepository.UpdateVerenigingen(body, Request.Body, cancellationToken);
        return Ok();
    }

    /// <summary>
    /// Zoek verenigingen op.
    /// </summary>
    /// <remarks>
    /// Dit endpoint laat toe verenigingen op te zoeken.
    ///
    /// Voor de zoekterm `q` kan je gebruik maken van volledige termen, of gebruik maken van wildcards.
    /// - `q=Liedekerke` zoekt in alle velden naar de volledige term,
    /// - `q=Liedeke*` zoekt in alle velden naar een term die begint met 'Liedeke',
    /// - `q=*kerke` zoekt in alle velden naar een term die eindigt op 'kerke',
    /// - `q=*kerke*` zoekt in alle velden naar een term die 'kerke' bevat.
    ///
    /// Om te zoeken binnen een bepaald veld, gebruik je de naam van het veld.
    /// - `q=hoofdlocatie:Liedekerke`
    /// - `q=korteNaam:DV*`
    ///
    /// Om te zoeken op een genest veld, beschrijf je het pad anar het veld.
    /// - `q=locaties.postcode:1000`
    ///
    /// Standaard gebruiken we een paginatie limiet van 50 verenigingen.
    /// Om een andere limiet te gebruiken, geef je de parameter `limit` mee.
    /// - `q=...&amp;limit=100`
    ///
    /// Om de volgende pagina's op te vragen, geef je de parameter `offset` mee.
    /// - `q=...&amp;offset=50`
    /// - `q=...&amp;offset=30&amp;limit=30`
    /// </remarks>
    /// <response code="200">Indien de zoekopdracht succesvol was.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("zoeken")]
    [ProducesResponseType(typeof(SearchVerenigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(contentType: WellknownMediaTypes.Json)]
    public async Task<IActionResult> Zoeken(
        [FromServices] ElasticClient elasticClient,
        [FromServices] SearchVerenigingenMapper mapper,
        [FromQuery] string q,
        [FromQuery] PaginationQueryParams paginationQueryParams)
    {
        var searchResponse = await Search(elasticClient, q, paginationQueryParams);

        var response = mapper.ToSearchVereningenResponse(searchResponse, paginationQueryParams);

        return Ok(response);
    }

    private static async Task<ISearchResponse<VerenigingDocument>> Search(
        IElasticClient client,
        string q,
        PaginationQueryParams paginationQueryParams)
        => await client.SearchAsync<VerenigingDocument>(
            s => s
                .From(paginationQueryParams.Offset)
                .Size(paginationQueryParams.Limit)
                .Query(
                    query => query.Bool(
                        boolQueryDescriptor => boolQueryDescriptor.Must(
                            queryContainerDescriptor => queryContainerDescriptor.QueryString(
                                queryStringQueryDescriptor => queryStringQueryDescriptor.Query(q)
                            )
                        )
                    )
                )
                .Aggregations(
                    aggregationContainerDescriptor => aggregationContainerDescriptor.Terms(
                        WellknownFacets.HoofdactiviteitenCountAggregateName,
                        valueCountAggregationDescriptor => valueCountAggregationDescriptor
                            .Field(document => document.Hoofdactiviteiten.Select(h => h.Code).Suffix("keyword"))
                            .Size(20)
                    )
                )
        );
}
