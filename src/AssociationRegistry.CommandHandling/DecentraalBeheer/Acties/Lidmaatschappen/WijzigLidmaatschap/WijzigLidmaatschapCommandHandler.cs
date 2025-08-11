namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Lidmaatschappen.WijzigLidmaatschap;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;

public class WijzigLidmaatschapCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public WijzigLidmaatschapCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(CommandEnvelope<WijzigLidmaatschapCommand> envelope, CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<VerenigingOfAnyKind>(
                VCode.Create(envelope.Command.VCode),
                envelope.Metadata);

        vereniging.WijzigLidmaatschap(envelope.Command.Lidmaatschap);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
