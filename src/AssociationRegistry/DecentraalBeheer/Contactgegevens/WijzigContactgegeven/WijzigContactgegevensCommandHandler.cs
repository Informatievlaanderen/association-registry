namespace AssociationRegistry.DecentraalBeheer.Contactgegevens.WijzigContactgegeven;

using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;

public class WijzigContactgegevenCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public WijzigContactgegevenCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigContactgegevenCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _verenigingRepository.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata.ExpectedVersion);

        var (contacgegevenId, waarde, beschrijving, isPrimair) = envelope.Command.Contactgegeven;
        vereniging.WijzigContactgegeven(contacgegevenId, waarde, beschrijving, isPrimair);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
