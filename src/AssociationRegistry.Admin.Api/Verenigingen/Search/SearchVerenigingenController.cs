namespace AssociationRegistry.Admin.Api.Verenigingen.Search;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using Exceptions;
using FluentValidation;
using Infrastructure;
using Infrastructure.Swagger.Annotations;
using Microsoft.AspNetCore.Mvc;
using Nest;
using RequestModels;
using ResponseModels;
using Schema.Search;
using Swashbuckle.AspNetCore.Filters;
using System.Text.RegularExpressions;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.Opvragen]
public class SearchVerenigingenController : ApiController
{
    private readonly ElasticClient _elasticClient;
    private readonly SearchVerenigingenResponseMapper _responseMapper;
    private readonly TypeMapping _typeMapping;

    private static readonly Func<SortDescriptor<VerenigingZoekDocument>, SortDescriptor<VerenigingZoekDocument>> DefaultSort =
        x => x.Descending(v => v.VCode);

    public SearchVerenigingenController(
        ElasticClient elasticClient,
        SearchVerenigingenResponseMapper responseMapper,
        TypeMapping typeMapping)
    {
        _elasticClient = elasticClient;
        _responseMapper = responseMapper;
        _typeMapping = typeMapping;
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
    ///
    ///     Om te zoeken binnen een bepaald veld, gebruik je de naam van het veld.
    ///     - `q=gemeente:Liedekerke`
    ///     - `q=korteNaam:DV*`
    ///
    ///     Om te zoeken op een genest veld, beschrijf je het pad naar het veld.
    ///     - `q=locaties.postcode:1000`
    ///
    ///     Standaard gebruiken we een paginatie limiet van 50 verenigingen.
    ///     Om een andere limiet te gebruiken, geef je de parameter `limit` mee.
    ///     De maximum limiet die kan gebruikt worden is 1000.
    ///     - `q=...&amp;limit=100`
    ///
    ///     Om de volgende pagina's op te vragen, geef je de parameter `offset` mee.
    ///     - `q=...&amp;offset=50`
    ///     - `q=...&amp;offset=30&amp;limit=30`
    ///
    ///     Er kan enkel gepagineerd worden binnen de eerste 1000 resultaten.
    ///     Dit betekent dat de som van limit en offset nooit meer kan bedragen dan 1000.
    ///
    ///     ### Sortering
    ///
    ///     Standaard wordt aflopend gesorteerd op vCode.
    ///     Wil je een eigen sortering meegeven, kan je gebruik maken van `sort=veldNaam`.
    ///     - Zonder `sort` parameter wordt standaard aflopend gesorteerd op `vCode`.
    ///     - `sort=naam` sorteert oplopend op `naam`.
    ///     - `sort=-naam` sorteert aflopend op `naam`.
    ///
    ///     Om te zoeken op een genest veld, beschrijf je het pad naar het veld.
    ///     - `sort=verenigingstype.code`
    ///
    ///     Om te sorteren op meerdere velden, combineer je de verschillende velden gescheiden door een komma.
    ///     - `sort=verenigingstype.code,-naam`
    ///
    ///     De volgende velden worden ondersteund voor gebruik bij het sorteren:
    ///     - `vCode`
    ///     - `verenigingstype.code`
    ///     - `verenigingstype.beschrijving`
    ///     - `roepnaam`
    ///     - `naam`
    ///     - `korteNaam`
    ///     - `doelgroep.minimumleeftijd`
    ///     - `doelgroep.maximumleeftijd`
    ///
    ///     Het gedrag van het sorteren op andere velden kan niet gegarandeerd worden.
    /// </remarks>
    /// <param name="q">De querystring</param>
    /// <param name="sort">De velden om op te sorteren</param>
    /// <param name="paginationQueryParams">De paginatie parameters</param>
    /// <param name="validator"></param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Indien de zoekopdracht succesvol was.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("zoeken")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(SearchVerenigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Zoeken(
        [FromQuery] string? q,
        [FromQuery] string? sort,
        [FromQuery] PaginationQueryParams paginationQueryParams,
        [FromServices] IValidator<PaginationQueryParams> validator,
        [FromServices] ILogger<SearchVerenigingenController> logger,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(paginationQueryParams, cancellationToken);
        q ??= "*";

        var searchResponse = await Search(_elasticClient, q, sort, paginationQueryParams, _typeMapping);

        if (searchResponse.ApiCall.HttpStatusCode == 400)
            return MapBadRequest(logger, searchResponse);

        if (!searchResponse.IsValid)
        {
            logger.LogError(searchResponse.OriginalException, searchResponse.DebugInformation);

            throw searchResponse.OriginalException;
        }

        var response = _responseMapper.ToSearchVereningenResponse(logger, searchResponse, paginationQueryParams, q);

        return Ok(response);
    }

    private IActionResult MapBadRequest(ILogger logger, ISearchResponse<VerenigingZoekDocument> searchResponse)
    {
        var match = Regex.Match(searchResponse.ServerError.Error.RootCause.First().Reason,
                                pattern: @"No mapping found for \[(.*).keyword\] in order to sort on");

        logger.LogError(searchResponse.OriginalException, message: "Fout bij het aanroepen van ElasticSearch");

        if (match.Success)
            throw new ZoekOpdrachtBevatOnbekendeSorteerVelden(match.Groups[1].Value);

        throw new ZoekOpdrachtWasIncorrect();
    }

    private static async Task<ISearchResponse<VerenigingZoekDocument>> Search(
        IElasticClient client,
        string q,
        string? sort,
        PaginationQueryParams paginationQueryParams,
        TypeMapping typemapping)
        => await client.SearchAsync<VerenigingZoekDocument>(
            s => s
                .From(paginationQueryParams.Offset)
                .Size(paginationQueryParams.Limit)
                .ParseSort(sort, DefaultSort, typemapping)
                .Query(query => query
                          .Bool(boolQueryDescriptor =>
                                    boolQueryDescriptor.Must(MatchWithQuery(q))
                                                       .MustNot(BeVerwijderd)
                           )
                 )
        );

    private static Func<QueryContainerDescriptor<VerenigingZoekDocument>, QueryContainer> MatchWithQuery(string q)
    {
        return queryContainerDescriptor =>
            queryContainerDescriptor.QueryString(
                queryStringQueryDescriptor
                    => queryStringQueryDescriptor.Query(q)
            );
    }

    private static QueryContainer BeVerwijderd(QueryContainerDescriptor<VerenigingZoekDocument> shouldDescriptor)
    {
        return shouldDescriptor
           .Term(termDescriptor
                     => termDescriptor
                       .Field(document => document.IsVerwijderd)
                       .Value(true));
    }
}
