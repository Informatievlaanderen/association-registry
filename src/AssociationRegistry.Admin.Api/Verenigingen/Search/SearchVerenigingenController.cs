namespace AssociationRegistry.Admin.Api.Verenigingen.Search;

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
using Projections.Search.Schema;
using RequestModels;
using ResponseModels;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using WellknownMediaTypes = Constants.WellknownMediaTypes;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Decentraal beheer van feitelijk verenigingen en afdelingen")]
public class SearchVerenigingenController : ApiController
{
    private readonly ElasticClient _elasticClient;
    private readonly SearchVerenigingenResponseMapper _responseMapper;

    public SearchVerenigingenController(ElasticClient elasticClient, SearchVerenigingenResponseMapper responseMapper)
    {
        _elasticClient = elasticClient;
        _responseMapper = responseMapper;
    }

    /// <summary>
    ///     Zoek verenigingen op.
    /// </summary>
    /// <remarks>
    ///     Dit endpoint laat toe verenigingen op te zoeken.
    ///     Voor de zoekterm `q` kan je gebruik maken van volledige termen, of gebruik maken van wildcards.
    ///     - `q=Liedekerke` zoekt in alle velden naar de volledige term,
    ///     - `q=Liedeke*` zoekt in alle velden naar een term die begint met 'Liedeke',
    ///     - `q=*kerke` zoekt in alle velden naar een term die eindigt op 'kerke',
    ///     - `q=*kerke*` zoekt in alle velden naar een term die 'kerke' bevat.
    ///     Om te zoeken binnen een bepaald veld, gebruik je de naam van het veld.
    ///     - `q=hoofdlocatie:Liedekerke`
    ///     - `q=korteNaam:DV*`
    ///     Om te zoeken op een genest veld, beschrijf je het pad naar het veld.
    ///     - `q=locaties.postcode:1000`
    ///     Standaard gebruiken we een paginatie limiet van 50 verenigingen.
    ///     Om een andere limiet te gebruiken, geef je de parameter `limit` mee.
    ///     - `q=...&amp;limit=100`
    ///     Om de volgende pagina's op te vragen, geef je de parameter `offset` mee.
    ///     - `q=...&amp;offset=50`
    ///     - `q=...&amp;offset=30&amp;limit=30`
    /// </remarks>
    /// <param name="q">De querystring</param>
    /// <param name="hoofdactiviteitenVerenigingsloket">De hoofdactiviteiten dewelke wel moeten meegenomen met de query, maar niet in de faccets te zien is.</param>
    /// <param name="paginationQueryParams">De paginatie parameters</param>
    /// <response code="200">Indien de zoekopdracht succesvol was.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("zoeken")]
    [ProducesResponseType(typeof(SearchVerenigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(WellknownMediaTypes.Json)]
    public async Task<IActionResult> Zoeken(
        [FromQuery] string? q,
        [FromQuery(Name = "facets.hoofdactiviteitenVerenigingsloket")]
        string? hoofdactiviteitenVerenigingsloket,
        [FromQuery] PaginationQueryParams paginationQueryParams)
    {
        q ??= "*";
        var hoofdActiviteitenArray = hoofdactiviteitenVerenigingsloket?.Split(separator: ',') ?? Array.Empty<string>();

        var searchResponse = await Search(_elasticClient, q, hoofdActiviteitenArray, paginationQueryParams);

        var response = _responseMapper.ToSearchVereningenResponse(searchResponse, paginationQueryParams, q, hoofdActiviteitenArray);

        return Ok(response);
    }

    private static async Task<ISearchResponse<VerenigingZoekDocument>> Search(
        IElasticClient client,
        string q,
        string[] hoofdactiviteiten,
        PaginationQueryParams paginationQueryParams)
        => await client.SearchAsync<VerenigingZoekDocument>(
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
                        GlobalAggregation(
                            agg,
                            agg2 =>
                                QueryFilterAggregation(
                                    agg2,
                                    q,
                                    HoofdactiviteitCountAggregation
                                )
                        )
                )
        );

    private static IAggregationContainer GlobalAggregation<T>(AggregationContainerDescriptor<T> agg, Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> aggregations) where T : class
    {
        agg.Global(
            WellknownFacets.GlobalAggregateName,
            d => d.Aggregations(
                aggregations
            )
        );
        return agg;
    }

    private static AggregationContainerDescriptor<T> QueryFilterAggregation<T>(AggregationContainerDescriptor<T> aggregationContainerDescriptor, string query, Func<AggregationContainerDescriptor<T>, IAggregationContainer> aggregations) where T : class
    {
        return aggregationContainerDescriptor.Filter(
            WellknownFacets.FilterAggregateName,
            aggregationDescriptor => aggregationDescriptor.Filter(
                    containerDescriptor => containerDescriptor.Bool(
                        queryDescriptor => queryDescriptor.Must(
                            m =>
                                m.QueryString(
                                    qs =>
                                        qs.Query(query)
                                )
                        )
                    )
                )
                .Aggregations(aggregations)
        );
    }

    private static AggregationContainerDescriptor<VerenigingZoekDocument> HoofdactiviteitCountAggregation(AggregationContainerDescriptor<VerenigingZoekDocument> aggregationContainerDescriptor)
    {
        return aggregationContainerDescriptor.Terms(
            WellknownFacets.HoofdactiviteitenCountAggregateName,
            valueCountAggregationDescriptor => valueCountAggregationDescriptor
                .Field(document => document.HoofdactiviteitenVerenigingsloket.Select(h => h.Code).Suffix("keyword"))
                .Size(size: 20)
        );
    }

    private static string BuildHoofdActiviteiten(IReadOnlyCollection<string> hoofdactiviteiten)
    {
        if (hoofdactiviteiten.Count == 0)
            return string.Empty;

        var builder = new StringBuilder();
        builder.Append(" AND (");

        foreach (var (hoofdactiviteit, index) in hoofdactiviteiten.Select((item, index) => (item, index)))
        {
            builder.Append($"hoofdactiviteitenVerenigingsloket.code:{hoofdactiviteit}");
            if (index < hoofdactiviteiten.Count - 1)
                builder.Append(" OR ");
        }

        builder.Append(value: ')');
        return builder.ToString();
    }
}
