namespace AssociationRegistry.CommandHandling.DecentraalBeheer.Acties.Vertegenwoordigers.VoegVertegenwoordigerToe;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Exceptions;
using Framework;
using Marten;
using Persoonsgegevens;

public class VoegVertegenwoordigerToeCommandHandler
{
    private readonly IVerenigingsRepository _repository;
    private readonly IDocumentStore _store;

    public VoegVertegenwoordigerToeCommandHandler(IVerenigingsRepository verenigingRepository, IDocumentStore store)
    {
        _repository = verenigingRepository;
        _store = store;
    }

    public async Task<EntityCommandResult> Handle(
        CommandEnvelope<VoegVertegenwoordigerToeCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _repository.Load<Vereniging>(envelope.Command.VCode, envelope.Metadata)
                                          .OrWhenUnsupportedOperationForType()
                                          .Throw<VerenigingMetRechtspersoonlijkheidKanGeenVertegenwoordigersToevoegen>();

        var refId = Guid.NewGuid();


        var vertegenwoordigerId = vereniging.VoegVertegenwoordigerToe(envelope.Command.Vertegenwoordiger, refId);

        var result = await _repository.Save(vereniging, envelope.Metadata, cancellationToken);

        return EntityCommandResult.Create(VCode.Create(envelope.Command.VCode), vertegenwoordigerId.VertegenwoordigerId, result);
    }
}
