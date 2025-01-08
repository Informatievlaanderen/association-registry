namespace AssociationRegistry.DecentraalBeheer.Vertegenwoordigers.VoegVertegenwoordigerToe;

using AssociationRegistry.Framework;
using AssociationRegistry.Vereniging;
using AssociationRegistry.Vereniging.Exceptions;

public class VoegVertegenwoordigerToeCommandHandler
{
    private readonly IVerenigingsRepository _repository;

    public VoegVertegenwoordigerToeCommandHandler(IVerenigingsRepository verenigingRepository)
    {
        _repository = verenigingRepository;
    }

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<VoegVertegenwoordigerToeCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _repository.Load<Vereniging>(envelope.Command.VCode, envelope.Metadata.ExpectedVersion)
                                          .OrWhenUnsupportedOperationForType()
                                          .Throw<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersToevoegen>();

        var vertegenwoordigerId = vereniging.VoegVertegenwoordigerToe(envelope.Command.Vertegenwoordiger);

        var result = await _repository.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), vertegenwoordigerId.VertegenwoordigerId, result);
    }
}
