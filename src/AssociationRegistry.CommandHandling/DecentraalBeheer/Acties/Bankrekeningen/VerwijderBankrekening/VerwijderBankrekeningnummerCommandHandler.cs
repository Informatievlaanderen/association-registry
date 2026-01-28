namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Bankrekeningen.VerwijderBankrekening;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using Framework;
using MartenDb.Store;

public class VerwijderBankrekeningnummerCommandHandler
{
    private readonly IAggregateSession _repository;

    public VerwijderBankrekeningnummerCommandHandler(IAggregateSession verenigingRepository)
    {
        _repository = verenigingRepository;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<VerwijderBankrekeningnummerCommand> envelope,
        CancellationToken cancellationToken = default
    )
    {
        var vereniging = await _repository.Load<Vereniging>(VCode.Create(envelope.Command.VCode), envelope.Metadata);

        vereniging.VerwijderBankrekeningnummer(envelope.Command.BankrekeningnummerId);

        var result = await _repository.Save(vereniging, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
