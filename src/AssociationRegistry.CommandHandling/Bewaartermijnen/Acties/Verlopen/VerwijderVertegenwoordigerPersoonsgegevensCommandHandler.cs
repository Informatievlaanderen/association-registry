namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Verlopen;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using Events;
using Framework;
using Grpc.Core;
using Marten.Events;
using MartenDb.Store;
using Persoonsgegevens;

public class VerwijderVertegenwoordigerPersoonsgegevensCommandHandler
{
    private readonly IVertegenwoordigerPersoonsgegevensRepository _persoonsgegevensRepository;
    private readonly IEventStore _store;

    public VerwijderVertegenwoordigerPersoonsgegevensCommandHandler(
        IVertegenwoordigerPersoonsgegevensRepository persoonsgegevensRepository,
        IEventStore store)
    {
        _persoonsgegevensRepository = persoonsgegevensRepository;
        _store = store;
    }

    public async Task Handle(
        CommandEnvelope<VerwijderVertegenwoordigerPersoonsgegevensCommand> enveloppe)
    {
        var vCode = enveloppe.Command.VCode;
        var vertegenwoordigerId = enveloppe.Command.VertegenwoordigerId;

        _persoonsgegevensRepository.Delete(vCode, vertegenwoordigerId);

        var bewaartermijnWerdVerlopen = new BewaartermijnWerdVerlopen(
            BewaartermijnId.CreateId(
                VCode.Create(vCode),
                BewaartermijnType.Vertegenwoordigers,
                vertegenwoordigerId),
            vCode,
            BewaartermijnType.Vertegenwoordigers.Value,
            vertegenwoordigerId);

        await _store.Save(bewaartermijnWerdVerlopen.BewaartermijnId,
                    1,
                    CommandMetadata.ForDigitaalVlaanderenProcess,
                    CancellationToken.None,
                    [bewaartermijnWerdVerlopen]);
    }
}
