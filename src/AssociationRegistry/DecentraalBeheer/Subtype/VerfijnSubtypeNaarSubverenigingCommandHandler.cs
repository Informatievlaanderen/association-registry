namespace AssociationRegistry.DecentraalBeheer.Subtype;

using Framework;
using Vereniging;

public class VerfijnSubtypeNaarSubverenigingCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public VerfijnSubtypeNaarSubverenigingCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerfijnSubtypeNaarSubverenigingCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<Vereniging>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata.ExpectedVersion);

        if (envelope.Command.SubverenigingVan.AndereVereniging is not null)
        {
            var verenigingMetRechtspersoonlijkheid = await _verenigingRepository.Load<VerenigingMetRechtspersoonlijkheid>(
                VCode.Create(envelope.Command.SubverenigingVan.AndereVereniging));

            envelope.Command.SubverenigingVan.AndereVerenigingNaam = verenigingMetRechtspersoonlijkheid.Naam;
        }

        vereniging.VerfijnNaarSubvereniging(envelope.Command.SubverenigingVan);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
