namespace AssociationRegistry.Public.Api.Verenigingen.Search;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using Detail;
using Exceptions;
using FluentValidation;
using Infrastructure;
using Infrastructure.ConfigurationBindings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nest;
using Queries;
using RequestModels;
using ResponseExamples;
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
    ///     ### Paginatie
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
    /// <param name="hoofdactiviteitenVerenigingsloket">
    ///     De hoofdactiviteiten dewelke wel moeten meegenomen met de query, maar
    ///     niet in de facets te zien is.
    /// </param>
    /// <param name="paginationQueryParams">De paginatie parameters</param>
    /// <param name="validator"></param>
    /// <param name="version">De versie van dit endpoint.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Indien de zoekopdracht succesvol was.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("zoeken")]
    [ProducesResponseType(typeof(SearchVerenigingenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(SearchVerenigingenResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(WellknownMediaTypes.Json)]
    public async Task<IActionResult> Zoeken(
        [FromQuery] string? q,
        [FromQuery] string? sort,
        [FromQuery(Name = "facets.hoofdactiviteitenVerenigingsloket")]
        string? hoofdactiviteitenVerenigingsloket,
        [FromQuery] PaginationQueryParams paginationQueryParams,
        [FromServices] IPubliekVerenigingenZoekQuery query,
        [FromServices] IValidator<PaginationQueryParams> validator,
        [FromServices] ILogger<SearchVerenigingenController> logger,
        [FromServices] AppSettings appsettings,
        [FromHeader(Name = WellknownHeaderNames.Version)] string? version,
        CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(paginationQueryParams, cancellationToken);

        q ??= "*";
        var hoofdActiviteitenArray = hoofdactiviteitenVerenigingsloket?.Split(separator: ',') ?? Array.Empty<string>();

        var searchResponse =
            await query.ExecuteAsync(new PubliekVerenigingenZoekFilter(q, sort, hoofdActiviteitenArray, paginationQueryParams),
                                     cancellationToken);

        if (searchResponse.ApiCall.HttpStatusCode == 400)
            return MapBadRequest(logger, searchResponse);

        if (!searchResponse.IsValid)
        {
            logger.LogWarning("An exception occurred while trying to search: {Info}", searchResponse.DebugInformation);
            logger.LogError(searchResponse.OriginalException, searchResponse.DebugInformation);

            throw searchResponse.OriginalException;
        }

        var responseMapper = new SearchVerenigingenResponseMapper(appsettings, logger, version);

        var response = responseMapper.ToSearchVereningenResponse(logger, searchResponse, paginationQueryParams, q, hoofdActiviteitenArray);
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
}
