namespace AssociationRegistry.Admin.Api.Verenigingen;

using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Constants;
using FluentValidation;
using Framework;
using Marten;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Projections;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class VerenigingenController : ApiController
{
    private readonly ISender _sender;

    public VerenigingenController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// Registreer een vereniging.
    /// </summary>
    [HttpPost]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerRequestExample(typeof(RegistreerVerenigingRequest), typeof(RegistreerVerenigingenRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetailsExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post(
        [FromServices] IValidator<RegistreerVerenigingRequest> validator,
        [FromBody] RegistreerVerenigingRequest request)
    {
        await DefaultValidatorExtensions.ValidateAndThrowAsync(validator, request);

        var command = new RegistreerVerenigingCommand(
            request.Naam,
            request.KorteNaam,
            request.KorteBeschrijving,
            request.StartDatum,
            request.KboNummer,
            request.ContactInfoLijst.Select(ToContactInfo),
            request.Locaties.Select(ToLocatie).ToArray());

        var metaData = new CommandMetadata(request.Initiator, SystemClock.Instance.GetCurrentInstant());
        var envelope = new CommandEnvelope<RegistreerVerenigingCommand>(command, metaData);
        await _sender.Send(envelope);
        return Accepted();
    }

    private static RegistreerVerenigingCommand.ContactInfo ToContactInfo(RegistreerVerenigingRequest.ContactInfo c)
        => new(c.Contactnaam, c.Email, c.Telefoon, c.Website, c.SocialMedia);

    private static RegistreerVerenigingCommand.Locatie ToLocatie(RegistreerVerenigingRequest.Locatie loc)
        => new(
            loc.Naam,
            loc.Straatnaam,
            loc.Huisnummer,
            loc.Busnummer,
            loc.Postcode,
            loc.Gemeente,
            loc.Land,
            loc.HoofdLocatie,
            loc.LocatieType);


    /// <summary>
    /// Vraag de historiek van een vereniging op.
    /// </summary>
    /// <response code="200">De historiek van een vereniging</response>
    /// <response code="404">De historiek van de gevraagde vereniging is niet gevonden</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpGet("{vCode}/historiek")]
    [ProducesResponseType(typeof(HistoriekResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(HistoriekResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [Produces(contentType: WellknownMediaTypes.Json)]
    public async Task<IActionResult> Historiek(
        [FromServices] IDocumentStore documentStore,
        [FromRoute] string vCode)
    {
        await using var session = documentStore.LightweightSession();
        var maybeHistoriekVereniging = await session.Query<VerenigingHistoriekDocument>()
            .Where(document => document.VCode == vCode)
            .SingleOrDefaultAsync();

        if (maybeHistoriekVereniging is not { } historiek)
            return NotFound();

        return Ok(
            new HistoriekResponse(
                vCode,
                historiek.Gebeurtenissen.Select(
                    gebeurtenis => new HistoriekGebeurtenisResponse(
                        gebeurtenis.Gebeurtenis,
                        gebeurtenis.Initiator,
                        gebeurtenis.Tijdstip)).ToList()));
    }
}
