namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens;

using System.Threading.Tasks;
using Infrastructure;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using EventStore;
using FluentValidation;
using Framework;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;
using Vereniging;
using Vereniging.WijzigBasisgegevens;
using Wolverine;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen/{vCode}")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
public class WijzigBasisgegevensController : ApiController
{
    private readonly IMessageBus _bus;
    private readonly AppSettings _appSettings;

    public WijzigBasisgegevensController(IMessageBus bus, AppSettings appSettings)
    {
        _bus = bus;
        _appSettings = appSettings;
    }

    /// <summary>
    /// Wijzig de basisgegevens van een vereniging.
    /// </summary>
    /// <remarks>
    /// Enkel velden die worden doorgestuurd in de request worden verwerkt. Null waarden worden niet verwerkt.
    /// Wanneer er wijzigingen veroorzaakt zijn door de request, bevat de response een sequence header.
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="vCode">De VCode van de vereniging</param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <param name="validator"></param>
    /// <response code="200">Er waren geen wijzigingen</response>
    /// <response code="202">De vereniging is aangepast</response>
    /// <response code="400">Er is een probleem met de doorgestuurde waarden. Zie body voor meet info.</response>
    /// <response code="404">De gevraagde vereniging is niet gevonden</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpPatch]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerRequestExample(typeof(WijzigBasisgegevensRequest), typeof(WijzigBasisgegevensRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetailsExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, "string", "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", "string", "De versie van de aangepaste vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "Location", "string", "De locatie van de aangepaste vereniging.")]
    public async Task<IActionResult> Patch(
        [FromServices] IValidator<WijzigBasisgegevensRequest> validator,
        [FromBody] WijzigBasisgegevensRequest? request,
        [FromRoute] string vCode,
        [FromHeader(Name = "If-Match")] string? ifMatch)
    {
        try
        {
            await validator.NullValidateAndThrowAsync(request);

            var command = request.ToWijzigBasisgegevensCommand(vCode);

            var metaData = new CommandMetadata(request.Initiator, SystemClock.Instance.GetCurrentInstant(), IfMatchParser.ParseIfMatch(ifMatch));
            var envelope = new CommandEnvelope<WijzigBasisgegevensCommand>(command, metaData);
            var wijzigResult = await _bus.InvokeAsync<CommandResult>(envelope);

            if (!wijzigResult.HasChanges()) return Ok();

            return this.AcceptedCommand(_appSettings, wijzigResult);
        }
        catch (UnexpectedAggregateVersionException)
        {
            return StatusCode(StatusCodes.Status412PreconditionFailed);
        }
    }
}
