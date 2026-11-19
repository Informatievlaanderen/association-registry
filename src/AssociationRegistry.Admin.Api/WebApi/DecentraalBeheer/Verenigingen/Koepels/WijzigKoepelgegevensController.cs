namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Koepels;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Examples;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using AssociationRegistry.Framework;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using CommandHandling.DecentraalBeheer.Acties.Koepels.WijzigKoepelgegevens;
using DecentraalBeheer.Vereniging;
using Examples;
using Extensions;
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
public class WijzigKoepelgegevensController : ApiController
{
    private readonly IMessageBus _messageBus;

    public WijzigKoepelgegevensController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    /// <summary>
    /// Wijzig de koepelgegevens van een vereniging.
    /// </summary>
    /// <remarks>
    /// Hiermee identificeert een vereniging zich als koepel (of expliciet niet als koepel)
    /// en legt een koepel de acceptatie van de privacyverklaring en de toelatingsvoorwaarden vast.
    /// Enkel de meegegeven velden worden gewijzigd.
    ///
    /// Enkel een vereniging met rechtspersoonlijkheid kan koepel zijn, en enkel een koepel kan de privacyverklaring accepteren.
    /// Wanneer de koepelstatus wijzigt naar `GeenKoepel` of `NietBepaald`, vervalt de acceptatie van de privacyverklaring.
    ///
    /// Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    /// Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    /// al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De VCode van de vereniging.</param>
    /// <param name="request">De te wijzigen koepelgegevens.</param>
    /// <param name="validator"></param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Er waren geen wijzigingen.</response>
    /// <response code="202">De koepelgegevens van de vereniging werden gewijzigd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPatch("{vCode}/koepelgegevens")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(WijzigKoepelgegevensRequest), typeof(WijzigKoepelgegevensRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedProblemDetailsExamples))]
    [SequenceResponseHeader()]
    [ETagResponseHeader("De versie van de geregistreerde vereniging.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> WijzigKoepelgegevens(
        [FromRoute] string vCode,
        [FromBody] WijzigKoepelgegevensRequest request,
        [FromServices] IValidator<WijzigKoepelgegevensRequest> validator,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null,
        CancellationToken cancellationToken = default
    )
    {
        await validator.NullValidateAndThrowAsync(request, cancellationToken: cancellationToken);

        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<WijzigKoepelgegevensCommand>(request.ToCommand(vCode), metaData);
        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope, cancellationToken);

        return this.PatchResponse(commandResult);
    }
}
