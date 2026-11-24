namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.BevestigRelatieaanvraag;

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
using CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.BevestigRelatieaanvraag;
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
public class BevestigRelatieaanvraagController : ApiController
{
    private readonly IMessageBus _messageBus;
    private readonly AppSettings _appSettings;

    public BevestigRelatieaanvraagController(IMessageBus messageBus, AppSettings appSettings)
    {
        _messageBus = messageBus;
        _appSettings = appSettings;
    }

    /// <summary>
    /// Bevestig een ontvangen relatieaanvraag.
    /// </summary>
    /// <remarks>
    /// De ontvanger van de aanvraag bevestigt ze, in de `vr-namens` header. De vereniging wordt dan lid of subvereniging
    /// van de koepel.
    ///
    /// Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    /// Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    /// al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="aanvraagId">De id van de relatieaanvraag.</param>
    /// <param name="request">De gegevens van de relatie. Alle velden zijn optioneel.</param>
    /// <param name="validator"></param>
    /// <param name="metadataProvider"></param>
    /// <param name="namens">De vCode van de vereniging die de aanvraag ontving.</param>
    /// <param name="ifMatch">De versie van de relatieaanvraag (optioneel).</param>
    /// <param name="cancellationToken"></param>
    /// <response code="202">De relatieaanvraag werd bevestigd.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De relatieaanvraag of de vereniging werd intussen gewijzigd.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost("{aanvraagId}/bevestiging")]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(BevestigRelatieaanvraagRequest), typeof(BevestigRelatieaanvraagRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SequenceResponseHeader()]
    [ETagResponseHeader("De versie van de relatieaanvraag.")]
    [LocationResponseHeader("De locatie van de bevestigde relatieaanvraag.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Bevestig(
        [FromRoute] string aanvraagId,
        [FromBody] BevestigRelatieaanvraagRequest request,
        [FromServices] IValidator<BevestigRelatieaanvraagRequest> validator,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = WellknownHeaderNames.Namens)] string? namens = null,
        [FromHeader(Name = WellknownHeaderNames.IfMatch)] string? ifMatch = null,
        CancellationToken cancellationToken = default
    )
    {
        var bevestigdDoor = VrNamensHeader.Verplicht(namens);

        await validator.NullValidateAndThrowAsync(request, cancellationToken: cancellationToken);

        var envelope = new CommandEnvelope<BevestigRelatieaanvraagCommand>(
            request.ToCommand(aanvraagId, bevestigdDoor),
            metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch))
        );

        var result = await _messageBus.InvokeAsync<RelatieaanvraagCommandResult>(envelope, cancellationToken);

        return this.PostResponse(_appSettings, result);
    }
}
