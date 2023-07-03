namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.FeitelijkeVereniging.WijzigContactgegeven;

using System.Threading.Tasks;
using AssociationRegistry.Acties.WijzigContactgegeven;
using Infrastructure;
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
[ApiRoute("verenigingen")]
[ApiExplorerSettings(GroupName = "Decentraal beheer van feitelijk verenigingen en afdelingen")]
public class WijzigContactgegevenController : ApiController
{
    private readonly IMessageBus _messageBus;
    private readonly IValidator<WijzigContactgegevenRequest> _validator;

    public WijzigContactgegevenController(IMessageBus messageBus, IValidator<WijzigContactgegevenRequest> validator)
    {
        _messageBus = messageBus;
        _validator = validator;
    }

    /// <summary>
    /// Wijzig een contactgegeven.
    /// </summary>
    /// <remarks>
    /// Na het uitvoeren van deze call wordt een sequentie teruggegeven via de `VR-Sequence` header.
    /// Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de zonet geregistreerde vereniging
    /// al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De unieke identificatie code van deze vereniging</param>
    /// <param name="contactgegevenId">De unieke identificatie code van dit contactgegeven binnen de vereniging</param>
    /// <param name="request">Het te wijzigen contactgegeven</param>
    /// <param name="initiator">Initiator header met als waarde de instantie die de wijziging uitvoert.</param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="200">Er waren geen wijzigingen.</response>
    /// <response code="202">De wijziging werd aanvaard.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPatch("{vCode}/contactgegevens/{contactgegevenId}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerRequestExample(typeof(WijzigContactgegevenRequest), typeof(WijzigContactgegevenRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetailsExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, "string", "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", "string", "De versie van de geregistreerde vereniging.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Patch(
        [FromRoute] string vCode,
        [FromRoute] int contactgegevenId,
        [FromBody] WijzigContactgegevenRequest request,
        [FromServices] InitiatorProvider initiator,
        [FromHeader(Name = "If-Match")] string? ifMatch = null)
    {
        await _validator.NullValidateAndThrowAsync(request);

        var metaData = new CommandMetadata(initiator, SystemClock.Instance.GetCurrentInstant(), IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<WijzigContactgegevenCommand>(request.ToCommand(vCode, contactgegevenId), metaData);
        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope);

        if (!commandResult.HasChanges()) return Ok();

        Response.AddSequenceHeader(commandResult.Sequence);
        Response.AddETagHeader(commandResult.Version);

        return Accepted();
    }
}
