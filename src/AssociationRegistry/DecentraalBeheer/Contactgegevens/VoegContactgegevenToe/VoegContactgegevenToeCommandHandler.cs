namespace AssociationRegistry.DecentraalBeheer.Contactgegevens.VoegContactgegevenToe;

using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;

public class VoegContactgegevenToeCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;

    public VoegContactgegevenToeCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _verenigingRepository = verenigingRepository;
    }

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<VoegContactgegevenToeCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging =
            await _verenigingRepository.Load<VerenigingOfAnyKind>(VCode.Create(envelope.Command.VCode), envelope.Metadata.ExpectedVersion);

       var contactgegeven = vereniging.VoegContactgegevenToe(envelope.Command.Contactgegeven);

        var result = await _verenigingRepository.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), contactgegeven.ContactgegevenId, result);
    }
}
