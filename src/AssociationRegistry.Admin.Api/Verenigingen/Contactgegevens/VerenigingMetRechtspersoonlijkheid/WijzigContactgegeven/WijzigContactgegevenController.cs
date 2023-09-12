﻿namespace AssociationRegistry.Admin.Api.Verenigingen.Contactgegevens.VerenigingMetRechtspersoonlijkheid.WijzigContactgegeven;

using Acties.WijzigContactgegeven;
using Acties.WijzigContactgegevenFromKbo;
using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Middleware;
using Infrastructure.Swagger;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Examples;
using FluentValidation;
using Framework;
using Infrastructure.ConfigurationBindings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RequestModels;
using Swashbuckle.AspNetCore.Filters;
using System.Threading.Tasks;
using Vereniging;
using Wolverine;
using ProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ProblemDetails;
using ValidationProblemDetails = Be.Vlaanderen.Basisregisters.BasicApiProblem.ValidationProblemDetails;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[SwaggerGroup.VerrijkingenVanKbo]
public class WijzigContactgegevenController : ApiController
{
    private readonly IMessageBus _messageBus;
    private readonly IValidator<WijzigContactgegevenRequest> _validator;
    private readonly AppSettings _appSettings;

    public WijzigContactgegevenController(
        IMessageBus messageBus,
        IValidator<WijzigContactgegevenRequest> validator,
        AppSettings appSettings)
    {
        _messageBus = messageBus;
        _validator = validator;
        _appSettings = appSettings;
    }

    /// <summary>
    ///     Wijzig een contactgegeven uit KBO.
    /// </summary>
    /// <remarks>
    ///     Na het uitvoeren van deze actie wordt een sequentie teruggegeven via de `VR-Sequence` header.
    ///     Deze waarde kan gebruikt worden in andere endpoints om op te volgen of de aanpassing
    ///     al is doorgestroomd naar deze endpoints.
    /// </remarks>
    /// <param name="vCode">De unieke identificatie code van deze vereniging</param>
    /// <param name="contactgegevenId">De unieke identificatie code van dit contactgegeven binnen de vereniging</param>
    /// <param name="request">Het te wijzigen contactgegeven</param>
    /// <param name="metadataProvider"></param>
    /// <param name="ifMatch">If-Match header met ETag van de laatst gekende versie van de vereniging.</param>
    /// <response code="200">Er waren geen wijzigingen.</response>
    /// <response code="202">De wijziging werd aanvaard.</response>
    /// <response code="400">Er was een probleem met de doorgestuurde waarden.</response>
    /// <response code="412">De gevraagde vereniging heeft niet de verwachte sequentiewaarde.</response>
    /// <response code="500">Er is een interne fout opgetreden.</response>
    [HttpPatch("{vCode}/kbo/contactgegevens/{contactgegevenId}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, WellknownHeaderNames.Sequence, "string", "Het sequence nummer van deze request.")]
    [SwaggerResponseHeader(StatusCodes.Status202Accepted, "ETag", "string", "De versie van de geregistreerde vereniging.")]
    [SwaggerRequestExample(typeof(WijzigContactgegevenRequest), typeof(WijzigContactgegevenRequestExamples))]
    [SwaggerResponseExample(StatusCodes.Status500InternalServerError, typeof(InternalServerErrorResponseExamples))]
    [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ProblemAndValidationProblemDetailsExamples))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Patch(
        [FromRoute] string vCode,
        [FromRoute] int contactgegevenId,
        [FromBody] WijzigContactgegevenRequest request,
        [FromServices] ICommandMetadataProvider metadataProvider,
        [FromHeader(Name = "If-Match")] string? ifMatch = null)
    {
        await _validator.NullValidateAndThrowAsync(request);

        var metaData = metadataProvider.GetMetadata(IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<WijzigContactgegevenFromKboCommand>(request.ToCommand(vCode, contactgegevenId), metaData);
        var commandResult = await _messageBus.InvokeAsync<CommandResult>(envelope);

        if (!commandResult.HasChanges()) return Ok();

        return this.AcceptedCommand(_appSettings, commandResult);
    }
}
