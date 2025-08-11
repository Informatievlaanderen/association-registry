namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.VerwijderLidmaatschap;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;

public class VerwijderLidmaatschapCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public VerwijderLidmaatschapCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerwijderLidmaatschapCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<VerenigingOfAnyKind>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata);

        vereniging.VerwijderLidmaatschap(envelope.Command.LidmaatschapId);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
