namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.TrekRelatieaanvraagIn;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure;
using AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Examples;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using AssociationRegistry.Admin.Api.WebApi.Verenigingen.Extensions;
using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.TrekRelatieaanvraagIn;
using DecentraalBeheer.Relatieaanvragen;
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
[ApiRoute("relatieaanvragen")]
[SwaggerGroup.DecentraalBeheer]
public class TrekRelatieaanvraagInController : ApiController
{
    private readonly IMessageBus _messageBus;
    private readonly AppSettings _appSettings;

    public TrekRelatieaanvraagInController(IMessageBus messageBus, AppSettings appSettings)
    {
        _messageBus = messageBus;
        _appSettings = appSettings;
    }

    /// <summary>
    /// Trek een ingediende relatieaanvraag in.
    /// </summary>
    /// <remarks>
    /// De aanvrager trekt de aanvraag in, in de `vr-namens` header. Er ontstaat geen relatie.
    ///
    /// Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    /// Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    /// al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="aanvraagId">De id van de relatieaanvraag.</param>
    /// <param name="request">De reden van de intrekking. Optioneel.</param>
    /// <param name="validator"></param>
    /// <param name="metadataProvider"></param>
    /// <param name="namens">De vCode van de vereniging die de aanvraag indiende.</param>
    /// <param name="ifMatch">De versie van de relatieaanvraag (optioneel).</param>
    /// <param name="cancellationToken"></param>
    /// <response code="202">De relatieaanvraag werd ingetrokken.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De relatieaanvraag werd intussen gewijzigd.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("{aanvraagId}/intrekking")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(TrekRelatieaanvraagInRequest), typeof(TrekRelatieaanvraagInRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SequenceResponseHeader()]
    [ETagResponseHeader("De versie van de relatieaanvraag.")]
    [LocationResponseHeader("De locatie van de ingetrokken relatieaanvraag.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> TrekIn(
        [FromRoute] string aanvraagId,
        [FromBody] TrekRelatieaanvraagInRequest request,
        [FromServices] IValidator<TrekRelatieaanvraagInRequest> validator,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = WellknownHeaderNames.Namens)] string? namens = null,
        [FromHeader(Name = WellknownHeaderNames.IfMatch)] string? ifMatch = null,
        CancellationToken cancellationToken = default
    )
    {
        var ingetrokkenDoor = VrNamensHeader.Verplicht(namens);

        await validator.NullValidateAndThrowAsync(request, cancellationToken: cancellationToken);

        var envelope = new CommandEnvelope<TrekRelatieaanvraagInCommand>(
            request.ToCommand(aanvraagId, ingetrokkenDoor),
            metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch))
        );

        var result = await _messageBus.InvokeAsync<RelatieaanvraagCommandResult>(envelope, cancellationToken);

        return this.PostResponse(_appSettings, result);
    }
}
