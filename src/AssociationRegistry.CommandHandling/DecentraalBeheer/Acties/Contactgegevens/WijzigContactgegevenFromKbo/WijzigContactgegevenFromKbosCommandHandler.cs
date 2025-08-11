namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Contactgegevens.WijzigContactgegevenFromKbo;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;

public class WijzigContactgegevenFromKboCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public WijzigContactgegevenFromKboCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigContactgegevenFromKboCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _verenigingRepository.Load<VerenigingMetRechtspersoonlijkheid>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata);

        var (contactgegevenId, beschrijving, isPrimair) = envelope.Command.Contactgegeven;
        vereniging.WijzigContactgegeven(contactgegevenId, beschrijving, isPrimair);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
