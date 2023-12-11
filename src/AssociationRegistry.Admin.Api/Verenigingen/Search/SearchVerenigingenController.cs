namespace AssociationRegistry.Admin.Api.Verenigingen.Search;

using System.Threading;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using FluentValidation;
using Infrastructure;
using Infrastructure.Swagger.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nest;
using RequestModels;
using ResponseModels;
using Schema.Search;
using Swashbuckle.AspNetCore.Filters;
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
        [FromQuery] PaginationQueryParams paginationQueryParams,
        [FromServices] IValidator<PaginationQueryParams> validator,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(paginationQueryParams, cancellationToken);
        q ??= "*";

        var searchResponse = await Search(_elasticClient, q, paginationQueryParams);

        var response = _responseMapper.ToSearchVereningenResponse(searchResponse, paginationQueryParams, q);

        return Ok(response);
    }

    private static async Task<ISearchResponse<VerenigingZoekDocument>> Search(
        IElasticClient client,
        string q,
        PaginationQueryParams paginationQueryParams)
        => await client.SearchAsync<VerenigingZoekDocument>(
            s => s
                .From(paginationQueryParams.Offset)
                .Size(paginationQueryParams.Limit)
                .Sort(x => x.Descending(v => v.VCode))
                .Query(
                     query => query.Bool(
                         boolQueryDescriptor => boolQueryDescriptor.Must(
                             queryContainerDescriptor => queryContainerDescriptor.QueryString(
                                 queryStringQueryDescriptor => queryStringQueryDescriptor.Query(q)
                             )
                         )
                     )
                 )
        );
}
