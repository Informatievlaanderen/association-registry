namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.DubbelDetectie;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.DuplicaatDetectie;
using Events.Factories;
using Framework;
using Marten;
using Microsoft.Extensions.Logging;
using ResultNet;

public class RapporteerDubbeleVerenigingenCommandHandler
{
    private readonly ILogger<RapporteerDubbeleVerenigingenCommandHandler> _logger;
    private readonly IDocumentSession _session;
    private readonly IDuplicateVerenigingsRepository _repository;

    public RapporteerDubbeleVerenigingenCommandHandler(
        IDuplicateVerenigingsRepository repository,
        IDocumentSession session,
        ILogger<RapporteerDubbeleVerenigingenCommandHandler> logger)
    {
        _repository = repository;
        _session = session;
        _logger = logger;
    }

    public async Task<Result> Handle(
        CommandEnvelope<RapporteerDubbeleVerenigingenCommand> message,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Handle {nameof(RapporteerDubbeleVerenigingenCommandHandler)} start");

        var command = message.Command;

        var @event = EventFactory.DubbeleVerenigingenWerdenGedetecteerd("DD0001", command.Naam, command.Locaties, command.GedetecteerdeDubbels);

        var result = await _repository.Save("DD0001", _session, message.Metadata, cancellationToken, [@event]);

        _logger.LogInformation($"Handle {nameof(RapporteerDubbeleVerenigingenCommandHandler)} end");

        return Result.Success();
    }
}
