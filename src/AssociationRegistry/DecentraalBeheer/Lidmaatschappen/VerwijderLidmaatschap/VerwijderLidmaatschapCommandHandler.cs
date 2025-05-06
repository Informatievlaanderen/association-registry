namespace AssociationRegistry.DecentraalBeheer.Lidmaatschappen.VerwijderLidmaatschap;

using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;

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
