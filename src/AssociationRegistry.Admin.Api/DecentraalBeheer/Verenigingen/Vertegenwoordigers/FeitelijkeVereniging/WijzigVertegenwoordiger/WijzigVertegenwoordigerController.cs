namespace AssociationRegistry.Admin.Api.Verenigingen.Vertegenwoordigers.FeitelijkeVereniging.WijzigVertegenwoordiger;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.Extensions;
using AssociationRegistry.Admin.Api.Infrastructure.Middleware;
using AssociationRegistry.Admin.Api.Infrastructure.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.Swagger.Examples;
using AssociationRegistry.Admin.Api.Infrastructure.Validation;
using AssociationRegistry.DecentraalBeheer.Vertegenwoordigers.WijzigVertegenwoordiger;
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
public class WijzigVertegenwoordigerController : ApiController
{
    private readonly IMessageBus _messageBus;
    private readonly IValidator<WijzigVertegenwoordigerRequest> _validator;

    public WijzigVertegenwoordigerController(IMessageBus messageBus, IValidator<WijzigVertegenwoordigerRequest> validator)
    {
        _messageBus = messageBus;
        _validator = validator;
    }

    /// <summary>
    ///     Wijzig een vertegenwoordiger.
    /// </summary>
    /// <remarks>
    ///     Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vertegenwoordigerId">De unieke identificatie code van deze vertegenwoordiger binnen de vereniging</param>
    /// <param name="request">De gegevens van de vertegenwoordiger die gewijzigd moeten worden</param>
    /// <param name="vCode">De unieke identificatie code van deze vereniging</param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="200">Er waren geen wijzigingen.</response>
    /// <response code="202">De vertegenwoordiger werd gewijzigd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPatch("{vCode}/vertegenwoordigers/{vertegenwoordigerId}")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(WijzigVertegenwoordigerRequest), typeof(WijzigVertegenwoordigerRequestExamples))]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, type: "string",
                           description: "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, name: "ETag", type: "string",
                           description: "De versie van de geregistreerde vereniging.")]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status412PreconditionFailed, typeof(PreconditionFailedProblemDetailsExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Patch(
        [FromRoute] string vCode,
        [FromRoute] int vertegenwoordigerId,
        [FromBody] WijzigVertegenwoordigerRequest request,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null)
    {
        await _validator.NullValidateAndThrowAsync(request);

        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<WijzigVertegenwoordigerCommand>(request.ToCommand(vCode, vertegenwoordigerId), metaData);
        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope);

        if (!commandResult.HasChanges()) return Ok();

        Response.AddSequenceHeader(commandResult.Sequence);
        Response.AddETagHeader(commandResult.Version);

        return Accepted();
    }
}
