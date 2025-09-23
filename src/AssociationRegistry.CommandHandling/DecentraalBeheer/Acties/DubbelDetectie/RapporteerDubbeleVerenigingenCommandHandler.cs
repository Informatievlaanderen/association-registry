namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.DubbelDetectie;

using AssociationRegistry.DecentraalBeheer.Vereniging.DubbelDetectie;
using Events.Factories;
using Framework;
using Marten;
using Microsoft.Extensions.Logging;
using ResultNet;

public class RapporteerDubbeleVerenigingenCommandHandler
{
    private readonly ILogger<RapporteerDubbeleVerenigingenCommandHandler> _logger;
    private readonly IDocumentSession _session;
    private readonly IDubbelDetectieVerenigingsRepository _repository;

    public RapporteerDubbeleVerenigingenCommandHandler(
        IDubbelDetectieVerenigingsRepository repository,
        IDocumentSession session,
        ILogger<RapporteerDubbeleVerenigingenCommandHandler> logger)
    {
        _repository = repository;
        _session = session;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CommandEnvelope<RapporteerDubbeleVerenigingenCommand> message,
        bool isNew,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(RapporteerDubbeleVerenigingenCommandHandler)} start");

        var command = message.Command;

        var @event = EventFactory.DubbeleVerenigingenWerdenGedetecteerd(message.Command.Key,command.Naam, command.Locaties, command.GedetecteerdeDubbels);

        if (isNew)
        {
            await _repository.SaveNew(message.Command.Key, _session, message.Metadata, cancellationToken, [@event]);
        }
        else
        {
            await _repository.Save(message.Command.Key, _session, message.Metadata, cancellationToken, [@event]);
        }

        _logger.LogInformation($"Handle {nameof(RapporteerDubbeleVerenigingenCommandHandler)} end");

        return Result.Success();
    }
}
