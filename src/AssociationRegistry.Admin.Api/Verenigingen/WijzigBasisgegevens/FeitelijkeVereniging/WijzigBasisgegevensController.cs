namespace AssociationRegistry.Admin.Api.Verenigingen.WijzigBasisgegevens.FeitelijkeVereniging;

using System.Threading.Tasks;
using AssociationRegistry.Acties.WijzigBasisgegevens;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.ConfigurationBindings;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Swashbuckle.AspNetCore.Filters;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Verenigingen")]
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
    ///     Wijzig de basisgegevens van een vereniging.
    /// </summary>
    /// <remarks>
    ///     Enkel velden die worden doorgestuurd in de request worden verwerkt. Null waarden worden niet verwerkt.
    ///     Wanneer er wijzigingen veroorzaakt zijn door de request, bevat de response een sequence header.
    /// </remarks>
    /// <param name="request"></param>
    /// <param name="vCode">De vCode van de vereniging</param>
    /// <param name="initiator">Initiator header met als waarde de instantie die de wijziging uitvoert.</param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <param name="validator"></param>
    /// <response code="200">Er waren geen wijzigingen</response>
    /// <response code="202">De vereniging is aangepast</response>
    /// <response code="400">Er is een probleem met de doorgestuurde waarden. Zie body voor meer info.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpPatch("{vCode}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerRequestExample(typeof(WijzigBasisgegevensRequest), typeof(WijzigBasisgegevensRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetailsExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, "string", "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", "string", "De versie van de aangepaste vereniging.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "Location", "string", "De locatie van de aangepaste vereniging.")]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Patch(
        [FromServices] IValidator<WijzigBasisgegevensRequest> validator,
        [FromBody] WijzigBasisgegevensRequest? request,
        [FromRoute] string vCode,
        [InitiatorHeader] string initiator,
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
