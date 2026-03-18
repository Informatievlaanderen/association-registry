namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.MaakValidatieBankrekeningnummerOngedaan;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Examples;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Extensions;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.ValideerBankrekening;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.MaakValidatieBankrekeningOngedaan;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.DecentraalBeheer]
public class MaakValidatieBankrekeningnummerOngedaanController : ApiController
{
    private readonly IMessageBus _messageBus;

    public MaakValidatieBankrekeningnummerOngedaanController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    ///     Maakt de validatie van een bankrekeningnummer ongedaan en zet deze terug naar niet-gevalideerd.
    /// </summary>

    /// <param name="vCode">De vCode van de vereniging.</param>
    /// <param name="bankrekeningnummerId">De unieke identificatie code van dit bankrekeningnummer binnen de vereniging.</param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="202">De validatie van het bankrekeningnummer werd ongedaan gemaakt.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpDelete("{vCode}/bankrekeningnummers/{bankrekeningnummerId:int}/validaties")]
    [ConsumesJson]
    [ProducesJson]
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
        description: "De versie van de geregistreerde vereniging."
    )]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post(
        [FromRoute] string vCode,
        [FromRoute] int bankrekeningnummerId,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null
    )
    {
        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<MaakValidatieBankrekeningnummerOngedaanCommand>(
            new MaakValidatieBankrekeningnummerOngedaanCommand(VCode.Create(vCode), bankrekeningnummerId),
            metaData
        );

        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope);

        return this.PostResponse(commandResult);
    }
}
