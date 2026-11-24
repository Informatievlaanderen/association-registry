namespace AssociationRegistry.Admin.Api.WebApi.Relatieaanvragen.DienRelatieaanvraagIn;

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
using CommandHandling.DecentraalBeheer.Acties.Relatieaanvragen.DienRelatieaanvraagIn;
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
public class DienRelatieaanvraagInController : ApiController
{
    private readonly IMessageBus _messageBus;
    private readonly AppSettings _appSettings;

    public DienRelatieaanvraagInController(IMessageBus messageBus, AppSettings appSettings)
    {
        _messageBus = messageBus;
        _appSettings = appSettings;
    }

    /// <summary>
    /// Dien een relatieaanvraag in.
    /// </summary>
    /// <remarks>
    /// Een koepel vraagt een vereniging om lid of subvereniging te worden, of een vereniging vraagt zelf om lid of
    /// subvereniging van een koepel te worden. De vereniging in de `vr-namens` header is de aanvrager: dat moet de koepel
    /// of de vereniging uit de aanvraag zijn.
    ///
    /// Er ontstaat enkel een aanvraag met status `Aangevraagd`, zonder relatie, subtypewijziging of beheerrecht.
    /// De aanvraag vervalt 90 dagen na de aanvraagdatum.
    ///
    /// Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    /// Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanvraag
    /// al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="request">De relatieaanvraag.</param>
    /// <param name="validator"></param>
    /// <param name="metadataProvider"></param>
    /// <param name="namens">De vCode van de vereniging die de aanvraag indient.</param>
    /// <param name="cancellationToken"></param>
    /// <response code="202">De relatieaanvraag werd ingediend.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPost]
    [ConsumesJson]
    [ProducesJson]
    [SwaggerRequestExample(typeof(DienRelatieaanvraagInRequest), typeof(DienRelatieaanvraagInRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [SequenceResponseHeader()]
    [ETagResponseHeader("De versie van de relatieaanvraag.")]
    [LocationResponseHeader("De locatie van de relatieaanvraag.")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DienIn(
        [FromBody] DienRelatieaanvraagInRequest request,
        [FromServices] IValidator<DienRelatieaanvraagInRequest> validator,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = WellknownHeaderNames.Namens)] string? namens = null,
        CancellationToken cancellationToken = default
    )
    {
        var aangevraagdDoor = VrNamensHeader.Verplicht(namens);

        await validator.NullValidateAndThrowAsync(request, cancellationToken: cancellationToken);

        var envelope = new CommandEnvelope<DienRelatieaanvraagInCommand>(
            request.ToCommand(aangevraagdDoor),
            metadataProvider.GetMetadata()
        );

        var result = await _messageBus.InvokeAsync<RelatieaanvraagCommandResult>(envelope, cancellationToken);

        return this.PostResponse(_appSettings, result);
    }
}
