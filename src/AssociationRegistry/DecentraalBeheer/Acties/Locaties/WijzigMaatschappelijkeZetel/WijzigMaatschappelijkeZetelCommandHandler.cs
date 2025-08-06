namespace AssociationRegistry.DecentraalBeheer.Acties.Locaties.WijzigMaatschappelijkeZetel;

using Framework;
using Vereniging;
using Vereniging.Geotags;

public class WijzigMaatschappelijkeZetelCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;
    private readonly IGeotagsService _geotagsService;

    public WijzigMaatschappelijkeZetelCommandHandler(IVerenigingsRepository verenigingRepository, IGeotagsService geotagsService)
    {
        _verenigingRepository = verenigingRepository;
        _geotagsService = geotagsService;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigMaatschappelijkeZetelCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<VerenigingMetRechtspersoonlijkheid>(VCode.Create(envelope.Command.VCode),
                                                                                 envelope.Metadata);

        vereniging.WijzigMaatschappelijkeZetel(
            envelope.Command.TeWijzigenLocatie.LocatieId,
            envelope.Command.TeWijzigenLocatie.Naam,
            envelope.Command.TeWijzigenLocatie.IsPrimair);

        await vereniging.HerberekenGeotags(_geotagsService);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
