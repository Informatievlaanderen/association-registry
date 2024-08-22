namespace AssociationRegistry.Admin.Api.Verenigingen.Historiek;

using Asp.Versioning;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using EventStore;
using Examples;
using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Swagger.Annotations;
using Infrastructure.Swagger.Examples;
using Marten;
using Microsoft.AspNetCore.Mvc;
using ResponseModels;
using Schema.Historiek;
using Swashbuckle.AspNetCore.Filters;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.Opvragen]
public class VerenigingenHistoriekController : ApiController
{
    private readonly VerenigingHistoriekResponseMapper _mapper;

    public VerenigingenHistoriekController(VerenigingHistoriekResponseMapper mapper)
    {
        _mapper = mapper;
    }

    /// <summary>
    ///     Vraag de historiek van een vereniging op.
    /// </summary>
    /// <remarks>
    /// De historiek van een vereniging geeft zicht op de wijzigingen op de verenigingsdata zoals terug te vinden in het register.
    ///
    /// De gebeurtenissen met naam “WerdGewijzigd” betekenen voor de basisgegevens het volgende:
    /// - data werd toegevoegd (een waarde werd toegevoegd na registratie van de vereniging)
    /// - data werd gewijzigd (de bestaande waarde werd gewijzigd)
    /// - data werd verwijderd (de waarde werd verwijderd)
    ///
    /// Contactgegevens, locaties en vertegenwoordigers maken geen onderdeel uit van de basisgegevens.
    /// Wijzigingen op deze data genereren gebeurtenissen met de namen “WerdToegevoegd”, “WerdGewijzigd” en “WerdVerwijderd”.
    /// </remarks>
    /// <param name="documentStore"></param>
    /// <param name="problemDetailsHelper"></param>
    /// <param name="vCode">De vCode van de vereniging</param>
    /// <param name="expectedSequence">Sequentiewaarde verkregen bij creatie of aanpassing vereniging.</param>
    /// <response code="200">De historiek van een vereniging</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="404">De historiek van de gevraagde vereniging is niet gevonden</response>
    /// <response code="412">De historiek van de gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpGet("{vCode}/historiek")]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HistoriekResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(NotFoundProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(HistoriekPreconditionFailedProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(typeof(HistoriekResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesJson]
    public async Task<IActionResult> Historiek(
        [FromServices] IDocumentStore documentStore,
        [FromServices] ProblemDetailsHelper problemDetailsHelper,
        [FromRoute] string vCode,
        [FromQuery] long? expectedSequence)
    {
        await using var session = documentStore.LightweightSession();

        if (!await documentStore.HasReachedSequence<BeheerVerenigingHistoriekDocument>(expectedSequence))
            throw new UnexpectedAggregateVersionException(ValidationMessages.Status412Historiek);

        var maybeHistoriekVereniging = await session.Query<BeheerVerenigingHistoriekDocument>()
                                                    .WithVCode(vCode)
                                                    .SingleOrDefaultAsync();

        if (maybeHistoriekVereniging is not { } historiek)
            return await Response.WriteNotFoundProblemDetailsAsync(problemDetailsHelper);

        return Ok(
            _mapper.Map(vCode, historiek));
    }
}
