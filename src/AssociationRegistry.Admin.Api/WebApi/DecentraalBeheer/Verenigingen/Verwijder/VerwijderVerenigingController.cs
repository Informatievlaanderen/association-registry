namespace AssociationRegistry.Admin.Api.WebApi.Verenigingen.Verwijder;

using Asp.Versioning;
using AssociationRegistry.Admin.Api.Infrastructure.CommandMiddleware;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Swagger.Annotations;
using AssociationRegistry.Admin.Api.Infrastructure.WebApi.Validation;
using AssociationRegistry.Framework;
using AssociationRegistry.Hosts.Configuration.ConfigurationBindings;
using AssociationRegistry.Vereniging;
using Be.Vlaanderen.Basisregisters.Api;
using CommandHandling.DecentraalBeheer.Acties.VerwijderVereniging;
using DecentraalBeheer.Vereniging;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RequestModels;
using Wolverine;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[ApiRoute("verenigingen")]
[ApiExplorerSettings(IgnoreApi = true)]
[Authorize(Policy = Program.SuperAdminPolicyName)]
public class VerwijderVerenigingController : ApiController
{
    private readonly IMessageBus _bus;
    private readonly AppSettings _appsettings;
    private readonly IValidator<VerwijderVerenigingRequest> _validator;

    public VerwijderVerenigingController(IMessageBus bus, AppSettings appsettings, IValidator<VerwijderVerenigingRequest> validator)
    {
        _bus = bus;
        _appsettings = appsettings;
        _validator = validator;
    }

    [HttpDelete("{vCode}")]
    [ConsumesJson]
    [ProducesJson]
    public async Task<IActionResult> Delete(
        [FromBody] VerwijderVerenigingRequest? request,
        [FromRoute] string vCode,
        [FromServices] ICommandMetadataProvider metadataProvider)
    {
        await _validator.NullValidateAndThrowAsync(request);

        var command = new VerwijderVerenigingCommand(VCode.Create(vCode), request.Reden);

        var metaData = metadataProvider.GetMetadata();
        var envelope = new CommandEnvelope<VerwijderVerenigingCommand>(command, metaData);
        var stopResult = await _bus.InvokeAsync<CommandResult>(envelope);

        if (!stopResult.HasChanges()) return Ok();

        return this.AcceptedCommand(_appsettings, stopResult);
    }
}
