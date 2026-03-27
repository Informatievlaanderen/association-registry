namespace AssociationRegistry.CommandHandling.Bewaartermijnen.Acties.Verlopen;

using AssociationRegistry.DecentraalBeheer.Vereniging;
using AssociationRegistry.DecentraalBeheer.Vereniging.Bewaartermijnen;
using Events;
using Framework;
using JasperFx.Events;
using Marten;
using NodaTime.Text;
using Persoonsgegevens;
using IEvent = Events.IEvent;

public class VerloopBewaartermijnCommandHandler
{
    private readonly IVertegenwoordigerPersoonsgegevensRepository _persoonsgegevensRepository;
    private readonly IDocumentSession _session;

    public VerloopBewaartermijnCommandHandler(
        IVertegenwoordigerPersoonsgegevensRepository persoonsgegevensRepository,
        IDocumentSession session
    )
    {
        _persoonsgegevensRepository = persoonsgegevensRepository;
        _session = session;
    }

    public async Task Handle(CommandEnvelope<VerloopBewaartermijnCommand> enveloppe)
    {
        var command = enveloppe.Command;

        SetMetadata();

        DeletePersoonsgegevens(command);
        AddVerlopenBewaartermijnVerlopen(command);
        AddPersoonsgegevensGeanonimiseerd(command);

        await _session.SaveChangesAsync();
    }

    private void DeletePersoonsgegevens(VerloopBewaartermijnCommand command) =>
        _persoonsgegevensRepository.Delete(command.VCode, command.VertegenwoordigerId);

    private void AddVerlopenBewaartermijnVerlopen(VerloopBewaartermijnCommand command)
    {
        var @event = new BewaartermijnWerdVerlopen(
            BewaartermijnId.CreateId(
                VCode.Create(command.VCode),
                PersoonsgegevensType.Vertegenwoordigers,
                command.VertegenwoordigerId
            ),
            command.VCode,
            PersoonsgegevensType.Vertegenwoordigers.Value,
            command.VertegenwoordigerId,
            command.Reden,
            command.Vervaldag
        );

        AppendEvents(aggregateId: @event.BewaartermijnId, events: [@event], expectedVersion: 1);
    }

    private void AddPersoonsgegevensGeanonimiseerd(VerloopBewaartermijnCommand command)
    {
        var @event = new VertegenwoordigerPersoonsgegevensWerdenGeanonimiseerd(
            command.VCode,
            command.VertegenwoordigerId,
            command.Reden,
            command.Vervaldag
        );

        AppendEvents(aggregateId: command.VCode, events: [@event], expectedVersion: null);
    }

    private void SetMetadata()
    {
        var metadata = CommandMetadata.ForDigitaalVlaanderenProcess;

        _session.SetHeader(MetadataHeaderNames.Initiator, metadata.Initiator);
        _session.SetHeader(MetadataHeaderNames.Tijdstip, InstantPattern.General.Format(metadata.Tijdstip));
        _session.CorrelationId = metadata.CorrelationId.ToString();

        if (metadata.AdditionalMetadata is null)
            return;

        foreach (var item in metadata.AdditionalMetadata.Items)
            item.ApplyTo(_session);
    }

    private StreamAction AppendEvents(string aggregateId, IReadOnlyCollection<IEvent> events, long? expectedVersion) =>
        expectedVersion is not null
            ? _session.Events.Append(aggregateId, expectedVersion.Value + events.Count, events)
            : _session.Events.Append(aggregateId, events);
}
