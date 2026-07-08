namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.WijzigBankrekening;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.Framework;
using MartenDb.Store;

public class WijzigBankrekeningnummerCommandHandler
{
    private readonly IAggregateSession _repository;

    public WijzigBankrekeningnummerCommandHandler(IAggregateSession verenigingRepository)
    {
        _repository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigBankrekeningnummerCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _repository.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata
        );

        vereniging.WijzigBankrekeningnummer(envelope.Command.Bankrekeningnummer);

        var result = await _repository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
