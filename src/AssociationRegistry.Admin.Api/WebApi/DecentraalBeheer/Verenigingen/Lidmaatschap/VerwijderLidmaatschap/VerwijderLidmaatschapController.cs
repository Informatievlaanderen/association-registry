namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Lidmaatschap.VerwijderLidmaatschap;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Examples;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.VerwijderLidmaatschap;
using DecentraalBeheer.Vereniging;
using Extensions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.DecentraalBeheer]
public class VerwijderLidmaatschapController : ApiController
{
    private readonly IMessageBus _messageBus;

    public VerwijderLidmaatschapController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Verwijder een lidmaatschap.
    /// </summary>
    /// <remarks>
    /// Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    /// Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    /// al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De VCode van de vereniging.</param>
    /// <param name="lidmaatschapId">De unieke identificator van het te verwijderen lidmaatschap.</param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="202">Het lidmaatschap werd verwijderd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpDelete("{vCode}/lidmaatschappen/{lidmaatschapId:int}")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedProblemDetailsExamples))]
    [SwaggerResponseHeader(
        StatusCodes.Status202Accepted,
        WellknownHeaderNames.Sequence,
        type: "string",
        description: "Het sequence nummer van deze request."
    )]
    [SwaggerResponseHeader(
        StatusCodes.Status202Accepted,
        name: "ETag",
        type: "string",
        description: "De versie van de verwijderde vereniging."
    )]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> VerwijderLidmaatschap(
        [FromRoute] string vCode,
        [FromRoute] int lidmaatschapId,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null
    )
    {
        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<VerwijderLidmaatschapCommand>(
            new VerwijderLidmaatschapCommand(VCode.Create(vCode), new LidmaatschapId(lidmaatschapId)),
            metaData
        );
        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope);

        return this.DeleteResponse(commandResult);
    }
}
