namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Bankrekeningnummers.VerwijderBankrekeningnummer;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Examples;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VoegBankrekeningToe;
using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VerwijderBankrekening;
using Extensions;
using FluentValidation;
using Hosts.Configuration.ConfigurationBindings;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.DecentraalBeheer]
public class VoegBankrekeningnummerToeController : ApiController
{
    private readonly IMessageBus _messageBus;
    private readonly AppSettings _appsettings;

    public VoegBankrekeningnummerToeController(IMessageBus messageBus, AppSettings appsettings)
    {
        _messageBus = messageBus;
        _appsettings = appsettings;
    }

    /// <summary>
    ///     Verwijder een Bankrekeningnummer.
    /// </summary>
    /// <remarks>
    ///     Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De vCode van de vereniging.</param>
    /// <param name="bankrekeningnummerId">De unieke identificatie code van dit bankrekeningnummer binnen de vereniging.</param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="202">Het bankrekeningnummer werd verwijderd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpDelete("{vCode}/bankrekeningnummers/{bankrekeningnummerId:int}")]
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
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(
        [FromRoute] string vCode,
        [FromRoute] int bankrekeningnummerId,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null
    )
    {
        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));

        var envelope = new CommandEnvelope<VerwijderBankrekeningnummerCommand>(
            new VerwijderBankrekeningnummerCommand(VCode.Create(vCode), bankrekeningnummerId),
            metaData
        );

        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope);

        return this.DeleteResponse(commandResult);
    }
}
