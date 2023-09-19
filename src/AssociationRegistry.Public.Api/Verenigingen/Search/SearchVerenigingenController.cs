namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Examples;
using Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using RequestModels;
using ResponseModels;
using Schema;
using Schema.Constants;
using Schema.Search;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Opvragen van verenigingen")]
public class SearchVerenigingenController : ApiController
{
    private readonly ElasticClient _elasticClient;
    private readonly SearchVerenigingenResponseMapper _responseMapper;

    private static readonly Func<SortDescriptor<VerenigingZoekDocument>, SortDescriptor<VerenigingZoekDocument>> DefaultSort =
        x => x.Descending(v => v.VCode);

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
    ///     - `q=gemeente:Liedekerke`
    ///     - `q=korteNaam:DV*`
    ///     Om te zoeken op een genest veld, beschrijf je het pad naar het veld.
    ///     - `q=locaties.postcode:1000`
    ///     Standaard gebruiken we een paginatie limiet van 50 verenigingen.
    ///     Om een andere limiet te gebruiken, geef je de parameter `limit` mee.
    ///     De maximum limiet die kan gebruikt worden is 1000.
    ///     - `q=...&amp;limit=100`
    ///     Om de volgende pagina's op te vragen, geef je de parameter `offset` mee.
    ///     - `q=...&amp;offset=50`
    ///     - `q=...&amp;offset=30&amp;limit=30`
    ///     Er kan enkel gepagineerd worden binnen de eerste 1000 resultaten.
    ///     Dit betekent dat de som van limit en offset nooit meer kan bedragen dan 1000.
    /// </remarks>
    /// <param name="q">De querystring</param>
    /// <param name="hoofdactiviteitenVerenigingsloket">
    ///     De hoofdactiviteiten dewelke wel moeten meegenomen met de query, maar
    ///     niet in de facets te zien is.
    /// </param>
    /// <param name="paginationQueryParams">De paginatie parameters</param>
    /// <param name="validator"></param>
    /// <param name="cancellationToken"></param>
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
        [FromQuery] string? sort,
        [FromQuery(Name = "facets.hoofdactiviteitenVerenigingsloket")]
        string? hoofdactiviteitenVerenigingsloket,
        [FromQuery] PaginationQueryParams paginationQueryParams,
        [FromServices] IValidator<PaginationQueryParams> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(paginationQueryParams, cancellationToken);

        q ??= "*";
        var hoofdActiviteitenArray = hoofdactiviteitenVerenigingsloket?.Split(separator: ',') ?? Array.Empty<string>();

        var searchResponse = await Search(_elasticClient, q, sort, hoofdActiviteitenArray, paginationQueryParams);

        if (searchResponse.ApiCall.HttpStatusCode == 400)
            return MapBadRequest(searchResponse);

        var response = _responseMapper.ToSearchVereningenResponse(searchResponse, paginationQueryParams, q, hoofdActiviteitenArray);

        return Ok(response);
    }

    private IActionResult MapBadRequest(
        ISearchResponse<VerenigingZoekDocument> searchResponse)
    {
        var match = Regex.Match(searchResponse.ServerError.Error.RootCause.First().Reason,
                                pattern: @"No mapping found for \[(.*).keyword\] in order to sort on");

        if (match.Success)
            throw new ZoekOpdrachtBevatOnbekendeSorteerVelden(match.Groups[1].Value);

        throw new ZoekOpdrachtWasIncorrect();
    }

    private static async Task<ISearchResponse<VerenigingZoekDocument>> Search(
        IElasticClient client,
        string q,
        string? sort,
        string[] hoofdactiviteiten,
        PaginationQueryParams paginationQueryParams)
        => await client.SearchAsync<VerenigingZoekDocument>(
            s =>
            {
                return s
                      .From(paginationQueryParams.Offset)
                      .Size(paginationQueryParams.Limit)
                      .ParseSort(sort, DefaultSort)
                      .Query(query => query
                                .Bool(boolQueryDescriptor => boolQueryDescriptor
                                                            .Must(queryContainerDescriptor
                                                                      => MatchQueryString(
                                                                          queryContainerDescriptor,
                                                                          $"{q}{BuildHoofdActiviteiten(hoofdactiviteiten)}"),
                                                                  BeActief
                                                             )
                                                            .MustNot(BeUitgeschrevenUitPubliekeDatastroom)
                                 )
                       )
                      .Aggregations(
                           agg =>
                               GlobalAggregation(
                                   agg,
                                   aggregations: agg2 =>
                                       QueryFilterAggregation(
                                           agg2,
                                           q,
                                           HoofdactiviteitCountAggregation
                                       )
                               )
                       );
            });

    private static IAggregationContainer GlobalAggregation<T>(
        AggregationContainerDescriptor<T> agg,
        Func<AggregationContainerDescriptor<T>, AggregationContainerDescriptor<T>> aggregations) where T : class
    {
        agg.Global(
            WellknownFacets.GlobalAggregateName,
            selector: d => d.Aggregations(
                aggregations
            )
        );

        return agg;
    }

    private static AggregationContainerDescriptor<T> QueryFilterAggregation<T>(
        AggregationContainerDescriptor<T> aggregationContainerDescriptor,
        string query,
        Func<AggregationContainerDescriptor<T>, IAggregationContainer> aggregations)
        where T : class, ICanBeUitgeschrevenUitPubliekeDatastroom, IHasStatus
    {
        return aggregationContainerDescriptor.Filter(
            WellknownFacets.FilterAggregateName,
            selector: aggregationDescriptor => aggregationDescriptor.Filter(
                                                                         containerDescriptor => containerDescriptor.Bool(
                                                                             queryDescriptor => queryDescriptor.Must(
                                                                                 m =>
                                                                                     MatchQueryString(m, query),
                                                                                 BeActief
                                                                             ).MustNot(BeUitgeschrevenUitPubliekeDatastroom)
                                                                         )
                                                                     )
                                                                    .Aggregations(aggregations)
        );
    }

    private static QueryContainer MatchQueryString<T>(QueryContainerDescriptor<T> m, string query)
        where T : class, ICanBeUitgeschrevenUitPubliekeDatastroom
    {
        return m.QueryString(
            qs =>
                qs.Query(query)
        );
    }

    private static AggregationContainerDescriptor<VerenigingZoekDocument> HoofdactiviteitCountAggregation(
        AggregationContainerDescriptor<VerenigingZoekDocument> aggregationContainerDescriptor)
    {
        return aggregationContainerDescriptor.Terms(
            WellknownFacets.HoofdactiviteitenCountAggregateName,
            selector: valueCountAggregationDescriptor => valueCountAggregationDescriptor
                                                        .Field(document => document.HoofdactiviteitenVerenigingsloket.Select(h => h.Code)
                                                                                   .Suffix("keyword"))
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

    private static QueryContainer BeUitgeschrevenUitPubliekeDatastroom<T>(QueryContainerDescriptor<T> q)
        where T : class, ICanBeUitgeschrevenUitPubliekeDatastroom
    {
        return q.Terms(t => t.Field(arg => arg.IsUitgeschrevenUitPubliekeDatastroom).Terms(true));
    }

    private static QueryContainer BeActief<T>(QueryContainerDescriptor<T> q)
        where T : class, IHasStatus
    {
        return q.Terms(t => t.Field(arg => arg.Status).Terms(VerenigingStatus.Actief));
    }
}
