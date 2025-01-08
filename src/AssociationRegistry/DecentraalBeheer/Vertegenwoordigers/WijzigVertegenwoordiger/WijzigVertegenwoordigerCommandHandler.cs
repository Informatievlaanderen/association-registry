namespace AssociationRegistry.DecentraalBeheer.Vertegenwoordigers.WijzigVertegenwoordiger;

using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;

public class WijzigVertegenwoordigerCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public WijzigVertegenwoordigerCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigVertegenwoordigerCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<VerenigingOfAnyKind>(VCode.Create(envelope.Command.VCode), envelope.Metadata.ExpectedVersion);

        var (vertegenwoordigerId, rol, roepnaam, email, telefoonNummer, mobiel, socialMedia, isPrimair) =
            envelope.Command.Vertegenwoordiger;

        vereniging.WijzigVertegenwoordiger(vertegenwoordigerId, rol, roepnaam, email, telefoonNummer, mobiel, socialMedia, isPrimair);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
