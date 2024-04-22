namespace AssociationRegistry.Acties.WijzigLocatie;

using Framework;
using Grar.AddressMatch;
using Marten;
using Vereniging;
using Wolverine.Marten;

public class WijzigLocatieCommandHandler
{
    private readonly IVerenigingsRepository _verenigingRepository;
    private readonly IMartenOutbox _outbox;
    private readonly IDocumentSession _session;

    public WijzigLocatieCommandHandler(
        IVerenigingsRepository verenigingRepository,
        IMartenOutbox outbox,
        IDocumentSession session
        )
    {
        _verenigingRepository = verenigingRepository;
        _outbox = outbox;
        _session = session;
    }

    public async Task<CommandResult> Handle(
        CommandEnvelope<WijzigLocatieCommand> envelope,
        CancellationToken cancellationToken = default)
    {
        var vereniging = await _verenigingRepository.Load<VerenigingOfAnyKind>(
            VCode.Create(envelope.Command.VCode),
            envelope.Metadata.ExpectedVersion);

        var (locatieId, locatietype, isPrimair, naam, adres, adresId) = envelope.Command.TeWijzigenLocatie;
        vereniging.WijzigLocatie(locatieId, naam, locatietype, isPrimair, adresId, adres);

        if (adres is not null || adresId is not null)
        {
            await _outbox.SendAsync(new TeSynchroniserenAdresMessage(envelope.Command.VCode, locatieId));
        }

        var result = await _verenigingRepository.Save(vereniging, _session, envelope.Metadata, cancellationToken);

        return CommandResult.Create(VCode.Create(envelope.Command.VCode), result);
    }
}
