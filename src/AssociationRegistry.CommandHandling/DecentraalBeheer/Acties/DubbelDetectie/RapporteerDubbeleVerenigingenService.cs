namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.DubbelDetectie;

using AssociationRegistry.DecentraalBeheer.Vereniging.DubbelDetectie;
using Events.Factories;
using Framework;
using Marten;
using Microsoft.Extensions.Logging;
using ResultNet;

public interface IRapporteerDubbeleVerenigingenService
{
    Task<Result> RapporteerAsync(
        CommandEnvelope<RapporteerDubbeleVerenigingenMessage> message,
        CancellationToken cancellationToken = default);
}

public class RapporteerDubbeleVerenigingenService : IRapporteerDubbeleVerenigingenService
{
    private readonly ILogger<RapporteerDubbeleVerenigingenService> _logger;
    private readonly IDocumentSession _session;
    private readonly IDubbelDetectieRepository _repository;

    public RapporteerDubbeleVerenigingenService(
        IDubbelDetectieRepository repository,
        IDocumentSession session,
        ILogger<RapporteerDubbeleVerenigingenService> logger)
    {
        _repository = repository;
        _session = session;
        _logger = logger;
    }

    public async Task<Result> RapporteerAsync(
        CommandEnvelope<RapporteerDubbeleVerenigingenMessage> message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(RapporteerDubbeleVerenigingenService)} start");

        var command = message.Command;

        var @event = EventFactory.DubbeleVerenigingenWerdenGedetecteerd(command.Bevestigingstoken,command.Naam, command.Locaties, command.GedetecteerdeDubbels);

        await _repository.SaveNew(Guid.NewGuid().ToString(), _session, message.Metadata, cancellationToken, [@event]);
        _logger.LogInformation($"Handle {nameof(RapporteerDubbeleVerenigingenService)} end");

        return Result.Success();
    }
}
