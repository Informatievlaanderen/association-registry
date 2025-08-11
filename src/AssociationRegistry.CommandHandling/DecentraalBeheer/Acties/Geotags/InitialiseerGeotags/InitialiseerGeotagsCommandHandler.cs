namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Geotags.InitialiseerGeotags;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Geotags;
using AssociationRegistry.Framework;

public class InitialiseerGeotagsCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;
    private readonly IGeotagsService _geotagsService;

    public InitialiseerGeotagsCommandHandler(IVerenigingsRepository verenigingRepository, IGeotagsService geotagsService)
    {
        _verenigingRepository = verenigingRepository;
        _geotagsService = geotagsService;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<InitialiseerGeotagsCommand> envelope, CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<VerenigingOfAnyKind>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata, allowVerwijderdeVereniging: true, allowDubbeleVereniging: true);

        await vereniging.InitialiseerGeotags(_geotagsService);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
