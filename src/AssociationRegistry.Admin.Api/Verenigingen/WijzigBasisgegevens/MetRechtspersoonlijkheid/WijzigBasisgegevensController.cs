namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.MetRechtspersoonlijkheid;

using System.Threading.Tasks;
using AssociationRegistry.Acties.VerenigingMetRechtspersoonlijkheid.WijzigBasisgegevens;
using Infrastructure;
using Infrastructure.ConfigurationBindings;
using Infrastructure.Extensions;
using Framework;
using Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using FluentValidation;
using Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen/kbo")]
[ApiExplorerSettings(GroupName = "Decentraal beheer van verenigingen met rechtspersoonlijkheid")]
public class WijzigBasisgegevensController : ApiController
{
    private readonly AppSettings _appSettings;
    private readonly IMessageBus _bus;

    public WijzigBasisgegevensController(IMessageBus bus, AppSettings appSettings)
    {
        _bus = bus;
        _appSettings = appSettings;
    }

    /// <summary>
    ///     Wijzig de basisgegevens.
    /// </summary>
    /// <remarks>
    ///     Enkel velden die worden doorgestuurd in de request worden verwerkt. Null waarden worden niet verwerkt.
    ///     Wanneer er wijzigingen veroorzaakt zijn door de request, bevat de response een sequence header.
    ///
    ///     Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="vCode">De vCode van de vereniging</param>
    /// <param name="initiator">Initiator header met als waarde de instantie die de wijziging uitvoert.</param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <param name="validator"></param>
    /// <response code="200">Er waren geen wijzigingen</response>
    /// <response code="202">De basisgegevens van de vereniging werden gewijzigd</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPatch("{vCode}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerRequestExample(typeof(WijzigBasisgegevensRequest), typeof(WijzigBasisgegevensRequestExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, "string", "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", "string", "De versie van de aangepaste vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "Location", "string", "De locatie van de aangepaste vereniging.")]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Patch(
        [FromServices] IValidator<WijzigBasisgegevensRequest> validator,
        [FromBody] WijzigBasisgegevensRequest? request,
        [FromRoute] string vCode,
        [FromServices] InitiatorProvider initiator,
        [FromHeader(Name = "If-Match")] string? ifMatch = null)
    {
        await validator.NullValidateAndThrowAsync(request);

        var command = request.ToCommand(vCode);

        var metaData = new CommandMetadata(initiator, SystemClock.Instance.GetCurrentInstant(), IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<WijzigBasisgegevensCommand>(command, metaData);
        var wijzigResult = await _bus.InvokeAsync<CommandResult>(envelope);

        if (!wijzigResult.HasChanges()) return Ok();

        return this.AcceptedCommand(_appSettings, wijzigResult);
    }
}
