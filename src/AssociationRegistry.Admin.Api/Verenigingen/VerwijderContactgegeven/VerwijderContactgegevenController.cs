namespace AssociationRegistry.Admin.Api.Verenigingen.VerwijderContactgegeven;

using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api;
using Framework;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Vereniging;
using Vereniging.VerwijderContactgegevens;
using Wolverine;

public class VerwijderContactgegevenController : ApiController
{
    private readonly IMessageBus _messageBus;

    public VerwijderContactgegevenController(IMessageBus messageBus)
    {
        _messageBus = messageBus;
    }

    public async Task<IActionResult> Delete(string vCode, int contactgegevenId, string? ifMatch = null)
    {
        var metaData = new CommandMetadata("", SystemClock.Instance.GetCurrentInstant(), IfMatchParser.ParseIfMatch(ifMatch));
        var envelope = new CommandEnvelope<VerwijderContactgegevenCommand>(new VerwijderContactgegevenCommand(), metaData);
        await _messageBus.InvokeAsync<CommandResult>(envelope);
        return Accepted();
    }
}
