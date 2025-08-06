namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Subtype;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Examples;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using AssociationRegistry.Admin.Api.Queries;
using AssociationRegistry.DecentraalBeheer.Subtype;
using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.DecentraalBeheer]
public class WijzigSubtypeController : ApiController
{
    private readonly IMessageBus _messageBus;

    public WijzigSubtypeController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    ///     Wijzig het subtype van een vereniging.
    /// </summary>
    /// <remarks>
    ///     Met deze actie kan het subtype van de vereniging worden verfijnd naar feitelijke vereniging,
    ///     subvereniging, of niet bepaald.
    ///
    ///     Bij het verfijnen naar subvereniging is het meegeven van aanvullende gegevens verplicht.
    ///     Deze aanvullende gegevens kunnen gewijzigd worden via dit endpoint, door opnieuw te
    ///     verfijnen naar subvereniging met de gewijzigde aanvullende gegevens.
    ///
    ///     Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De VCode van de vereniging</param>
    /// <param name="request"></param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="200">Er waren geen wijzigingen.</response>
    /// <response code="202">Het subtype van de vereniging werd aangepast.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPatch("{vCode}/subtype")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(WijzigSubtypeRequest), typeof(WijzigSubtypeRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedProblemDetailsExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, type: "string",
                           description: "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "ETag", type: "string",
                           description: "De versie van de aangepaste vereniging.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> WijzigSubtype(
        [FromRoute] string vCode,
        [FromBody] WijzigSubtypeRequest request,
        [FromServices] IGetNamesForVCodesQuery namesForVCodesQuery,
        [FromServices] IValidator<WijzigSubtypeRequest> validator,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null,
        CancellationToken cancellationToken = default)
    {
        await validator.NullValidateAndThrowAsync(request, cancellationToken);

        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));

        var gewenstSubtype = VerenigingssubtypeCode.Parse(request.Subtype);
        CommandResult? commandResult = null;
        if (gewenstSubtype.IsFeitelijkeVereniging)
        {
            var command = request.ToVerfijnSubtypeNaarFeitelijkeVerenigingCommand(vCode);
            return await Handle(command, metaData, cancellationToken);
        }

        if (gewenstSubtype.IsNietBepaald)
        {
            var command = request.ToZetSubtypeTerugNaarNietBepaaldCommand(vCode);
            return await Handle(command, metaData, cancellationToken);
        }

        if (gewenstSubtype.IsSubVereniging)
        {
            var command = request.ToWijzigSubtypeCommand(vCode);
            var envelope = new CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand>(command, metaData);
            commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope, cancellationToken);
        }

        if (!commandResult.HasChanges())
            return Ok();

        Response.AddSequenceHeader(commandResult.Sequence);
        Response.AddETagHeader(commandResult.Version);

        return Accepted();
    }

    public async Task<IActionResult> Handle<TCommand>(TCommand command, CommandMetadata metaData, CancellationToken cancellationToken = default)
    {
        var envelope = new CommandEnvelope<TCommand>(command, metaData);
        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope, cancellationToken);

        if (!commandResult.HasChanges())
            return Ok();

        Response.AddSequenceHeader(commandResult.Sequence);
        Response.AddETagHeader(commandResult.Version);

        return Accepted();
    }
}
