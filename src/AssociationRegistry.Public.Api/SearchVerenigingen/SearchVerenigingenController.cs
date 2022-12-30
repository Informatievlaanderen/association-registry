namespace AssociationRegistry.Public.Api.SearchVerenigingen;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
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
        [FromQuery] string? q,
        [FromQuery(Name = "facets.hoofdactiviteiten")] string? hoofdactiviteiten,
        [FromQuery] PaginationQueryParams paginationQueryParams)
    {
        q ??= "*";
        var hoofdActiviteitenArray = hoofdactiviteiten?.Split(',') ?? Array.Empty<string>();

        var searchResponse = await Search(elasticClient, q, hoofdActiviteitenArray, paginationQueryParams);

        var response = mapper.ToSearchVereningenResponse(searchResponse, paginationQueryParams, q, hoofdActiviteitenArray);

        return Ok(response);
    }

    private static async Task<ISearchResponse<VerenigingDocument>> Search(
        IElasticClient client,
        string q,
        string[] hoofdactiviteiten,
        PaginationQueryParams paginationQueryParams)
        => await client.SearchAsync<VerenigingDocument>(
            s => s
                .From(paginationQueryParams.Offset)
                .Size(paginationQueryParams.Limit)
                .Query(
                    query => query.Bool(
                        boolQueryDescriptor => boolQueryDescriptor.Must(
                            queryContainerDescriptor => queryContainerDescriptor.QueryString(
                                queryStringQueryDescriptor => queryStringQueryDescriptor.Query($"{q}{BuildHoofdActiviteiten(hoofdactiviteiten)}")
                            )
                        )
                    )
                )
                .Aggregations(
                    agg =>
                        agg.Global(
                            WellknownFacets.GlobalAggregateName,
                            d => d.Aggregations(
                                gagg => gagg.Filter(
                                    WellknownFacets.FilterAggregateName,
                                    aggregationDescriptor => aggregationDescriptor.Filter(
                                        containerDescriptor => containerDescriptor.Bool(
                                            queryDescriptor => queryDescriptor.Must(
                                                m => m.QueryString(qs => qs.Query(q))))).Aggregations(
                                        agg => agg.Terms(
                                            WellknownFacets.HoofdactiviteitenCountAggregateName,
                                            valueCountAggregationDescriptor => valueCountAggregationDescriptor
                                                .Field(document => document.Hoofdactiviteiten.Select(h => h.Code).Suffix("keyword"))
                                                .Size(20)
                                        ))))))
        );

    private static string BuildHoofdActiviteiten(IReadOnlyCollection<string> hoofdactiviteiten)
    {
        if (hoofdactiviteiten.Count == 0)
            return string.Empty;

        var builder = new StringBuilder();
        builder.Append(" AND (");

        foreach (var (hoofdactiviteit, index) in hoofdactiviteiten.Select((item, index) => (item, index)))
        {
            builder.Append($"hoofdactiviteiten.code:{hoofdactiviteit}");
            if (index < hoofdactiviteiten.Count - 1)
                builder.Append(" OR ");
        }

        builder.Append(')');
        return builder.ToString();
    }
}
