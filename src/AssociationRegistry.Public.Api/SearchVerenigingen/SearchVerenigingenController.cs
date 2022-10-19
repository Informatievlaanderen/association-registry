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
    private const string HoofdactiviteitenCountAggregateName = "Count_By_Hoofdactiviteit";

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
    /// <response code="200">Indien de zoekopdracht succesvol was.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("zoeken")]
    [ProducesResponseType(typeof(SearchVerenigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(contentType: WellknownMediaTypes.Json)]
    public async Task<IActionResult> Zoeken([FromServices] ElasticClient elasticClient, [FromQuery] string q)
    {
        var searchResponse = await Search(q, elasticClient);

        var response = ToSearchVereningenResponse(searchResponse);

        return Ok(response);
    }

    private static SearchVerenigingenResponse ToSearchVereningenResponse(ISearchResponse<VerenigingDocument> searchResponse)
    {
        var verenigingen = searchResponse.Hits.Select(
            x =>
                new Vereniging(
                    x.Source.VCode,
                    x.Source.Naam,
                    x.Source.KorteNaam,
                    x.Source.Hoofdactiviteiten.ToImmutableArray(),
                    x.Source.Hoofdlocatie,
                    x.Source.Doelgroep,
                    x.Source.Locaties.Select(locatie => new Locatie(string.Empty, string.Empty, locatie)).ToImmutableArray(),
                    x.Source.Activiteiten.Select(activiteit => new Activiteit(-1, activiteit)).ToImmutableArray()
                )).ToImmutableArray();

        var hoofdactiviteitenCountAggregate = searchResponse.Aggregations.Terms(HoofdactiviteitenCountAggregateName);
        var facets = hoofdactiviteitenCountAggregate
            .Buckets
            .ToImmutableDictionary(
                bucket => bucket.Key.ToString(),
                bucket => bucket.DocCount??0);
        return new SearchVerenigingenResponse(verenigingen, facets);
    }

    private static async Task<ISearchResponse<VerenigingDocument>> Search(string q, IElasticClient client)
    {
        return await client.SearchAsync<VerenigingDocument>(
            s => s
                .From(0)
                .Size(10)
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
                        HoofdactiviteitenCountAggregateName,
                        valueCountAggregationDescriptor => valueCountAggregationDescriptor
                            .Field(document => document.Hoofdactiviteiten.Suffix("keyword"))
                            .Size(20)
                    )
                )
        );
    }
}
