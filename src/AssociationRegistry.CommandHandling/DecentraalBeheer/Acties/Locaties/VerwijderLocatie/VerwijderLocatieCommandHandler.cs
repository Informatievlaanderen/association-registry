namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Locaties.VerwijderLocatie;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;

public class VerwijderLocatieCommandHandler
{
    private readonly IVerenigingsRepository _repository;
    private readonly IGeotagsService _geotagsService;

    public VerwijderLocatieCommandHandler(IVerenigingsRepository repository, IGeotagsService geotagsService)
    {
        _repository = repository;
        _geotagsService = geotagsService;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<VerwijderLocatieCommand> message, CancellationToken cancellationToken = default)
    {
        var vereniging = await _repository.Load<VerenigingOfAnyKind>(VCode.Create(message.Command.VCode), message.Metadata);

        vereniging.VerwijderLocatie(message.Command.LocatieId);

        await vereniging.HerberekenGeotags(_geotagsService);

        var result = await _repository.Save(vereniging, message.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(message.Command.VCode), result);
    }
}
