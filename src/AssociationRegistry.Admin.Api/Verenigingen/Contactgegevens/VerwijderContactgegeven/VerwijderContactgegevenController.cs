﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerwijderContactgegeven;

using System.Threading.Tasks;
using Acties.VerwijderContactgegeven;
using Infrastructure;
using Framework;
using Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Infrastructure.Extensions;
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
[ApiExplorerSettings(GroupName = "Vereniging Contactgegevens")]
public class VerwijderContactgegevenController : ApiController
{
    private readonly IMessageBus _messageBus;

    public VerwijderContactgegevenController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Verwijder een contactgegeven.
    /// </summary>
    /// <remarks>
    /// Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    /// Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    /// al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De VCode van de vereniging</param>
    /// <param name="contactgegevenId">Id van het contactgegeven</param>
    /// <param name="initiator">Initiator header met als waarde de instantie die de wijziging uitvoert.</param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="202">Het contactgegeven werd verwijderd.</response>
    /// <response code="400">Er is een probleem met de doorgestuurde waarden. Zie body voor meer info.</response>
    /// <response code="500">Als er een interne fout is opgetreden.</response>
    [HttpDelete("{vCode}/contactgegevens/{contactgegevenId:int}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ValidationProblemDetailsExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, "string", "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", "string", "De versie van de geregistreerde vereniging.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(EmptyResult), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> Delete(
        [FromRoute] string vCode,
        [FromRoute] int contactgegevenId,
        [InitiatorHeader] string initiator,
        [IfMatchHeader] string? ifMatch = null)
    {
        var metaData = new CommandMetadata(initiator, SystemClock.Instance.GetCurrentInstant(), IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<VerwijderContactgegevenCommand>(
            new VerwijderContactgegevenCommand(VCode.Create(vCode), contactgegevenId),
            metaData);

        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope);

        Response.AddSequenceHeader(commandResult.Sequence);
        Response.AddETagHeader(commandResult.Version);
        return Accepted();
    }
}
