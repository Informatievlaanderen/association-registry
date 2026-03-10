namespace AssociationRegistry.Events;

using NodaTime;

public record BewaartermijnWerdVerlopen(
    string BewaartermijnId,
    string VCode,
    string BewaartermijnType,
    int RecordId,
    Instant Vervaldag
) : IEvent;
